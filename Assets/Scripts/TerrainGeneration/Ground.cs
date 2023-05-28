using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Generation _generation;
    [SerializeField] GroundTile[] _tiles;

    [Header("Generation")]
    [SerializeField] [Range(0.01f, 10)] float _biomeVariation;
    [SerializeField] [Range(0.01f, 10)] float _heightVariation;
    [SerializeField] float _maxX;
    [SerializeField] float _maxY;

    [SerializeField] WeightedTile[] _tilePrefabs;
    float _totalBiomeWeight;

    public Dictionary<Vector2Int, GroundTile> groundMap;

    [Header("Settings")]
    [SerializeField] [Range(0.1f, 5f)] float _cellSize;
    [SerializeField] Color _gizmosColor;

    private float TOTAL_TILES;
    private FastNoiseLite noiseGenerator;
    public float cellSize
    {
        get
        {
            return _cellSize;
        }
    }

    const float RootOf2 = 1.41421356f;
    const float MinimumFloatValue = 0.01f;

    enum Generation
    {
        GetFromScene,
        Random
    }

    private void Awake()
    {
        noiseGenerator = new FastNoiseLite();
        noiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        TOTAL_TILES = _maxX * _maxY;
        switch (_generation)
        {
            case Generation.GetFromScene:
                break;

            case Generation.Random:
                GenerateMap();
                break;

            default:
                break;
        }

        SetGroundGridFromScene();

        foreach (var target in FindObjectsOfType<Target>())
        {
            GroundTile tile;

            if (groundMap.TryGetValue(ToCellCoords(target.transform.position), out tile))
            {
                target.SetCurrentGroundTile(tile);
            }
        }
    }

    void GenerateMap()
    {
        SetGroundGridFromScene();

        float baseX = UnityEngine.Random.Range(-_maxX, _maxX);
        float basey = UnityEngine.Random.Range(-_maxY, _maxY);

        float[,] falloffMap = new float[(int)_maxX, (int)_maxY];
        for (int i = 0; i < _maxX; i++) 
        {
            for (int j = 0; j < _maxY; j++) 
            {
                float iv = i / _maxX * 2 - 1;
                float jv = j / _maxY * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(iv), Mathf.Abs(jv));
                float val = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
                falloffMap[i, j] = val;
            }
        }

        foreach (var prefab in _tilePrefabs) 
        {
            _totalBiomeWeight += prefab.biomeWeight;
        }

        for (int i = 0; i < _maxX; i++)
        {
            for (int j = 0; j < _maxY; j++)
            {
                GroundTile outTile;
                Vector2Int v = new Vector2Int(i, j);
                if (groundMap.TryGetValue(v, out outTile) == false)
                {
                    float noiseValue = Mathf.PerlinNoise(baseX + i * _heightVariation, basey + j * _heightVariation);
                    float biomeNoise = noiseGenerator.GetNoise(baseX + i * _biomeVariation, basey + j * _biomeVariation);
                    BiomeType biome;
                    var tile = GetTileBiome(biomeNoise- falloffMap[i, j], out biome);
                    Debug.Log(biome);
                    tile.GetComponent<GroundTile>().TerrainType = GetTilePrefab(noiseValue - falloffMap[i, j], (int)biome);
                    //Debug.Log("noise:" + noiseValue + " " + "biomeNoise: " + biomeNoise + " " + "FalloffValue: " + falloffMap[i, j]);
                    tile.GetComponent<GroundTile>().AssignMaterial();
                    var newTile = Instantiate(tile, new Vector3(i * _cellSize, tile.transform.position.y * _cellSize, j * _cellSize), Quaternion.identity);

                    newTile.transform.parent = transform;
                    outTile = newTile.GetComponent<GroundTile>();
                    groundMap.Add(v, outTile);
                }
            }
        }
    }

    private int CalculateTotalHeightWeight(int biome) 
    {
        int totalHeightWeight = 0;
        for (int i = 0; i < _tilePrefabs[biome].heightWeight.Length; i++)
        {
            totalHeightWeight += _tilePrefabs[biome].heightWeight[i];
        }
        return totalHeightWeight;
    }
    private void GenerateRivers() 
    {
    
    }
    GameObject GetTileBiome(float value, out BiomeType biome)
    {
        float biomeThreshhold = 0;
        for (int i = 0; i < _tilePrefabs.Length; i++) 
        {
            biomeThreshhold += _tilePrefabs[i].biomeWeight / _totalBiomeWeight;
            if (value < biomeThreshhold)
            {
                biome = _tilePrefabs[i].tilePrefab.GetComponent<GroundTile>().BiomeType;
                return _tilePrefabs[i].tilePrefab;
            }
        }
        biome = _tilePrefabs[_tilePrefabs.Length - 1].tilePrefab.GetComponent<GroundTile>().BiomeType;
        return _tilePrefabs[_tilePrefabs.Length-1].tilePrefab;
    }

    TerrainType GetTilePrefab(float value, int biome)
    {
        float accumulatedWeight = 0;
        for (int i = 0; i < _tilePrefabs.Length; i++)
        {
            accumulatedWeight += _tilePrefabs[biome].heightWeight[i] / CalculateTotalHeightWeight(biome);
            Debug.Log("b: " + (BiomeType)biome + " " + _totalBiomeWeight + " h:" + CalculateTotalHeightWeight(biome));
            if (value < accumulatedWeight && _tilePrefabs[biome].heightWeight[i] != 0)
            {
                Debug.Log((TerrainType)i);
                return (TerrainType)i;
            }
        }
        return 0;
    }


    [ContextMenu("Set Ground Grid")]
    void SetGroundGridFromScene()
    {
        _tiles = GetComponentsInChildren<GroundTile>();
        groundMap = new Dictionary<Vector2Int, GroundTile>();

        if (_tiles != null && _tiles.Length != 0)
        {
            for (int i = 0; i < _tiles.Length; i++)
            {
                var cellCoord = ToCellCoords(_tiles[i].transform.position);
                _tiles[i].SetCellCoord(cellCoord);

                _tiles[i].name = "Cell (" + cellCoord.x + ", " + cellCoord.y + ")";
                groundMap.Add(_tiles[i].cellCoord, _tiles[i]);
                TOTAL_TILES++;
            }

            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i].GetNeighBours(groundMap);
            }
        }

    }

    public Vector2Int ToCellCoords(Vector3 worldPosition)
    {
        Vector3 v = worldPosition;

        v /= _cellSize;

        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmosColor;
        for (int i = 0; i < _maxX; i++)
        {
            for (int j = 0; j < _maxY; j++)
            {
                Gizmos.DrawWireCube(new Vector3(_cellSize * i, 0, _cellSize * j), new Vector3(_cellSize, _cellSize, _cellSize));
            }
        }
    }


    [Serializable]
    struct WeightedTile
    {
        public GameObject tilePrefab;
        public int[] heightWeight;
        public int biomeWeight;
    }
}

