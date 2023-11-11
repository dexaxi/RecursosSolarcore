using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Highlightable))]
public class GroundTile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public BiomeType BiomeType;

    [Range(0, 10)] public int weight = 1;
    public Vector2Int CellCoord { get; private set; }
    public Highlightable Highlightable { get; private set; }

    [HideInInspector]
    public bool isClosed;
    [HideInInspector]
    public float gCost;
    [HideInInspector]
    public GroundTile parent;

    Vector3 _originalPosition;
    Vector3 _introStartingPosition;

    [SerializeField] Vector2 _cordsOffset;
    [SerializeField] [Range(-25, -1)] float _introDepth;
    [SerializeField] [Range(1, 20)] float _introSpeed;

    public Material[] terrainMaterials; //hacky, must be improved;

    public GroundTile[] neighbours;
    private void Awake()
    {
        Highlightable = GetComponent<Highlightable>();
    }

    void Start()
    {
        IntroAnimation();
    }
    void IntroAnimation()
    {
        _originalPosition = transform.position;
        _introStartingPosition = _originalPosition + new Vector3(0, 0.1f + _cordsOffset.x * Mathf.Abs(CellCoord.x) + _cordsOffset.y * Mathf.Abs(CellCoord.y), 0) * _introDepth;

        StartCoroutine(IntroAnimationCoroutine());
    }

    IEnumerator IntroAnimationCoroutine()
    {
        float elapsedTime = 0;

        float introTime = Vector3.Distance(_introStartingPosition, _originalPosition) / _introSpeed;

        while (elapsedTime < introTime)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(_introStartingPosition, _originalPosition, elapsedTime / introTime);

            yield return null;
        }

        transform.position = _originalPosition;
    }

    public void SetCellCoord(Vector2Int v)
    {
        CellCoord = v;
    }

    public List<GroundTile> GetNeighBours(Dictionary<Vector2Int, GroundTile> groundGrid)
    {
        List<GroundTile> newNeighbours = new List<GroundTile>();
        GroundTile neighbour;

        for (int i = -1; i < 2; i++)
        {
            if (i == 0) continue;
            if (groundGrid.TryGetValue(CellCoord + new Vector2Int(i, 0), out neighbour))
            {
                newNeighbours.Add(neighbour);
            }
        }

        for (int i = -1; i < 2; i++)
        {
            if (i == 0) continue;
            if (groundGrid.TryGetValue(CellCoord + new Vector2Int(0, i), out neighbour))
            {
                newNeighbours.Add(neighbour);
            }
        }

        neighbours = newNeighbours.ToArray();
        return newNeighbours;
    }

    public List<GroundTile> GetHexagonalNeighBours(Dictionary<Vector2Int, GroundTile> groundGrid)
    {
        List<GroundTile> newNeighbours = new List<GroundTile>();
        GroundTile neighbour;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;
                if (groundGrid.TryGetValue(CellCoord + new Vector2Int(i, j), out neighbour))
                {
                    newNeighbours.Add(neighbour);
                }
            }
        }

        this.neighbours = newNeighbours.ToArray();
        return newNeighbours;
    }

}
