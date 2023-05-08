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
    [SerializeField] [Range(0.01f, 2)] float _variation;
    [SerializeField] float _maxX;
    [SerializeField] float _maxY;

    [SerializeField] WeightedTile[] _tilePrefabs;
    float _totalWeight;

    public Dictionary<Vector2Int, GroundTile> groundMap;

    [Header("Settings")]
    [SerializeField] [Range(0.1f, 5f)] float _cellSize;
    [SerializeField] Color _gizmosColor;

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

        foreach (var prefab in _tilePrefabs)
        {
            _totalWeight += prefab.weight;
        }

        for (int i = 0; i < _maxX; i++)
        {
            for (int j = 0; j < _maxY; j++)
            {
                GroundTile outTile;
                if (groundMap.TryGetValue(new Vector2Int(i, j), out outTile) == false)
                {
                    float noiseValue = Mathf.PerlinNoise(baseX + i * _variation, basey + j * _variation);

                    var tile = GetTilePrefab(noiseValue);

                    var newTile = Instantiate(tile, new Vector3(i * _cellSize, 0, j * _cellSize), Quaternion.identity);

                    newTile.transform.parent = transform;
                }
            }

        }
    }

    GameObject GetTilePrefab(float value)
    {
        float accumulatedWeight = 0;

        for (int i = 0; i < _tilePrefabs.Length; i++)
        {
            accumulatedWeight += _tilePrefabs[i].weight / _totalWeight;

            if (value < accumulatedWeight)
            {
                Debug.Log(i);
                return _tilePrefabs[i].tilePrefab;
            }

        }

        return _tilePrefabs[_tilePrefabs.Length - 1].tilePrefab;
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
        public int weight;
    }
}

