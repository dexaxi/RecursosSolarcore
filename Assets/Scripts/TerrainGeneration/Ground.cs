using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Generation Generated;
    [SerializeField] GroundTile[] GroundTiles;

    [Header("Generation")]
    [SerializeField] [Range(0.01f, 10)] float BiomeVariation;
    [SerializeField] [Range(0.01f, 10)] float HeightVariation;
    [SerializeField] int MaxX;
    [SerializeField] int MaxY;
    [SerializeField] [Range(0.01f, 1)] float MinBiomeProportion;

    [SerializeField] WeightedTile[] TilePrefabs;

    public Dictionary<Vector2Int, GroundTile> GroundMap;

    [Header("Settings")]
    [SerializeField] [Range(0.1f, 5f)] float CellSize;
    [SerializeField] Color GizmoColor;

    private float _totalBiomeWeight;
    private int _totalTiles;
    private FastNoiseLite _noiseGenerator;
    private int _minTilesPerBiome;

    public float cellSize
    {
        get
        {
            return CellSize;
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
        _noiseGenerator = new FastNoiseLite();
        _noiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        _totalTiles = MaxX * MaxY; 
        _minTilesPerBiome = (int)(MinBiomeProportion * (_totalTiles / TilePrefabs.Length));

        switch (Generated)
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

            if (GroundMap.TryGetValue(ToCellCoords(target.transform.position), out tile))
            {
                target.SetCurrentGroundTile(tile);
            }
        }
    }

    void GenerateMap()
    {
        if(GroundMap != null) GroundMap.Clear();
        SetGroundGridFromScene();

        float baseX = UnityEngine.Random.Range(-MaxX, MaxX);
        float basey = UnityEngine.Random.Range(-MaxY, MaxY);

        foreach (var prefab in TilePrefabs) 
        {
            _totalBiomeWeight += prefab.biomeWeight;
        }

        for (int i = 0; i < MaxX; i++)
        {
            for (int j = 0; j < MaxY; j++)
            {
                GroundTile outTile;
                Vector2Int v = new Vector2Int(i, j);
                if (GroundMap.TryGetValue(v, out outTile) == false)
                {
                    float noiseValue = Mathf.PerlinNoise(baseX + i * HeightVariation, basey + j * HeightVariation);
                    float biomeNoise = _noiseGenerator.GetNoise(baseX + i * BiomeVariation, basey + j * BiomeVariation);
                    var tile = GetTileBiome(biomeNoise);
                    tile.transform.position = new Vector3(tile.transform.position.x, GetTileHeight(noiseValue) ,tile.transform.position.z);
                    var newTile = Instantiate(tile, new Vector3(i * CellSize, tile.transform.position.y * CellSize, j * CellSize), Quaternion.identity);
                    newTile.transform.parent = transform;
                    outTile = newTile.GetComponent<GroundTile>();
                    GroundMap.Add(v, outTile);
                }
            }
        }

        RegenerateIfNotFeasable();

    }

    private void GenerateRivers() 
    {
        
    }

    private void RegenerateIfNotFeasable() 
    {
        for (int i = 0; i < TilePrefabs.Length; i++)
        {
            if (TilePrefabs[i].spawnCount < _minTilesPerBiome)
            {
                Debug.Log("Regenerating because of Biome: " + (BiomeType)i + " spawnCount: " + TilePrefabs[i].spawnCount + " minSpawnCount: " + _minTilesPerBiome);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void Regenerate() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    GameObject GetTileBiome(float value)
    {
        float biomeThreshhold = 0;
        for (int i = 0; i < TilePrefabs.Length; i++) 
        {
            biomeThreshhold += TilePrefabs[i].biomeWeight / _totalBiomeWeight;
            if (value < biomeThreshhold && TilePrefabs[i].biomeWeight != 0)
            {
                TilePrefabs[i].spawnCount++;
                return TilePrefabs[i].tilePrefab;
            }
        }

        Debug.LogWarning("Warning! No suitable biome found.");
        TilePrefabs[^1].spawnCount++;
        return TilePrefabs[^1].tilePrefab;
    }

    int GetTileHeight(float value)
    {
        Debug.LogWarning("Warning! No Valid Terrain Type Found.");
        return 0;
    }


    [ContextMenu("Set Ground Grid")]
    void SetGroundGridFromScene()
    {
        GroundTiles = GetComponentsInChildren<GroundTile>();
        GroundMap = new Dictionary<Vector2Int, GroundTile>();

        if (GroundTiles != null && GroundTiles.Length != 0)
        {
            for (int i = 0; i < GroundTiles.Length; i++)
            {
                var cellCoord = ToCellCoords(GroundTiles[i].transform.position);
                GroundTiles[i].SetCellCoord(cellCoord);

                GroundTiles[i].name = "Cell (" + cellCoord.x + ", " + cellCoord.y + ")";
                GroundMap.Add(GroundTiles[i].CellCoord, GroundTiles[i]);
                _totalTiles++;
            }

            for (int i = 0; i < GroundTiles.Length; i++)
            {
                GroundTiles[i].GetNeighBours(GroundMap);
            }
        }

    }

    public Vector2Int ToCellCoords(Vector3 worldPosition)
    {
        Vector3 v = worldPosition;

        v /= CellSize;

        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        for (int i = 0; i < MaxX; i++)
        {
            for (int j = 0; j < MaxY; j++)
            {
                Gizmos.DrawWireCube(new Vector3(CellSize * i, 0, CellSize * j), new Vector3(CellSize, CellSize, CellSize));
            }
        }
    }


    [Serializable]
    struct WeightedTile
    {
        public GameObject tilePrefab;
        public int biomeWeight;
        public int spawnCount;
    }
}

