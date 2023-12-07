using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Ground : MonoBehaviour
{
    public static Ground Instance;

    [Header("References")]
    [SerializeField] Generation Generated;
    [SerializeField] GroundTile[] GroundTiles;

    [Header("Generation")]
    [SerializeField] [Range(0.01f, 10)] float BiomeVariation;
    [SerializeField] [Range(0.01f, 10)] float HeightVariation;
    [SerializeField] int MaxX;
    [SerializeField] int MaxY;
    [SerializeField] [Range(0.01f, 1)] float MinBiomeProportion;

    public Dictionary<Vector2Int, GroundTile> GroundMap;

    [Header("Settings")]
    [SerializeField] [Range(0.1f, 5f)] float CellSize;
    [SerializeField] Color GizmoColor;

    private Biome[] _biomeScriptableObjects;
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

    [HideInInspector]
    public bool HasLoaded { get; private set; }

    const float RootOf2 = 1.41421356f;
    const float MinimumFloatValue = 0.01f;

    enum Generation
    {
        GetFromScene,
        Random
    }

    private void Awake()
    {

        if (Instance == null) Instance = this;
        else DestroyImmediate(Instance);

        _noiseGenerator = new FastNoiseLite();
        _noiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        _totalTiles = MaxX * MaxY;
        HasLoaded = false;

        BiomeHandler.Instance.AddBiomeFilter(BiomeType.Greenland);
        BiomeHandler.Instance.AddBiomeFilter(BiomeType.Water);
        BiomeHandler.Instance.AddBiomeFilter(BiomeType.Rocky);
        BiomeHandler.Instance.AddBiomeFilter(BiomeType.Desert);
        StartMapGeneration();
    }

    public void StartMapGeneration()
    {
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
    }

    void GenerateMap()
    {
        GroundMap?.Clear();
        SetGroundGridFromScene();

        float baseX = UnityEngine.Random.Range(-MaxX, MaxX);
        float basey = UnityEngine.Random.Range(-MaxY, MaxY);

        foreach (var prefab in _biomeScriptableObjects) 
        {
            _totalBiomeWeight += prefab.biomeWeight;
        }

        for (int i = 0; i < MaxX; i++)
        {
            for (int j = 0; j < MaxY; j++)
            {
                GroundTile outTile;
                Vector2Int v = new(i, j);
                if (GroundMap.TryGetValue(v, out _) == false)
                {
                    float noiseValue = Mathf.PerlinNoise(baseX + i * HeightVariation, basey + j * HeightVariation);
                    float biomeNoise = _noiseGenerator.GetNoise(baseX + i * BiomeVariation, basey + j * BiomeVariation);
                    var biome = GetTileBiome(biomeNoise);
                    var tile = biome.tilePrefab;
                    tile.transform.position = new Vector3(tile.transform.position.x, GetTileHeight(biome.Type) ,tile.transform.position.z);
                    var newTile = Instantiate(tile, new Vector3(i * CellSize, tile.transform.position.y * CellSize, j * CellSize), Quaternion.identity);
                    newTile.transform.parent = transform;
                    outTile = newTile.GetComponent<GroundTile>();
                    outTile.SetBiome(biome);
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
        for (int i = 0; i < _biomeScriptableObjects.Length; i++)
        {
            if (_biomeScriptableObjects[i].spawnCount < _minTilesPerBiome)
            {
                Debug.Log("Regenerating because of Biome: " + (BiomeType)i + " spawnCount: " + _biomeScriptableObjects[i].spawnCount + " minSpawnCount: " + _minTilesPerBiome);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        Debug.Log("Map loaded successfully.");
        HasLoaded = true;
    }

    public void Regenerate() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    Biome GetTileBiome(float value)
    {
        float biomeThreshhold = 0;
        for (int i = 0; i < _biomeScriptableObjects.Length; i++) 
        {
            biomeThreshhold += _biomeScriptableObjects[i].biomeWeight / _totalBiomeWeight;
            if (value < biomeThreshhold && _biomeScriptableObjects[i].biomeWeight != 0)
            {
                _biomeScriptableObjects[i].spawnCount++;
                return _biomeScriptableObjects[i];
            }
        }
        Debug.LogWarning("Warning! No suitable biome found.");
        _biomeScriptableObjects[^1].spawnCount++;
        return _biomeScriptableObjects[^1];
    }

    float GetTileHeight(BiomeType type)
    {
        switch (type)
        {
            default:
                return 0;
            case BiomeType.Water:
                return -0.3f;
        }
    }


    [ContextMenu("Set Ground Grid")]
    void SetGroundGridFromScene()
    {
        GroundTiles = GetComponentsInChildren<GroundTile>();
        GroundMap = new Dictionary<Vector2Int, GroundTile>();

        _biomeScriptableObjects = BiomeHandler.Instance.GetFilteredBiomes().ToArray();
        _minTilesPerBiome = (int)(MinBiomeProportion * (_totalTiles / _biomeScriptableObjects.Length));

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

    public Vector2Int FindEmptyCellCoords() 
    {
        GroundTile[] tiles = FindObjectsOfType<GroundTile>();
        foreach (GroundTile tile in tiles) 
        {
            if (!tile.isClosed) return ToCellCoords(tile.transform.position);
        }

        return Vector2Int.zero;
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

    private int[,] RotateMatrixCounterClockwise(int[,] oldMatrix)
    {
        int[,] newMatrix = new int[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
        int newColumn, newRow = 0;
        for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
        {
            newColumn = 0;
            for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
            {
                newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
                newColumn++;
            }
            newRow++;
        }
        return newMatrix;
    }

    private int[,] RotateMatrixNTimes(int[,] oldMatrix, int times)
    {
        int[,] returnMatrix = oldMatrix;
        while (times > 0) 
        {
            returnMatrix = RotateMatrixCounterClockwise(returnMatrix);
            times--;
        }
        return returnMatrix;
    }

    public GroundTile GetTileFromCellCoords(Vector2Int cellCoords) 
    {
        return GroundMap[cellCoords];
    }

    public GroundTile GetTileFromCellCoords(int x, int y) 
    {
        return GroundMap[new Vector2Int(x, y)];
    }

}

