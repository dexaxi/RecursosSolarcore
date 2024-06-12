using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Highlightable))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GroundTile : MonoBehaviour
{
    [field: SerializeField] public Vector2Int CellCoord { get; private set; }
    public Highlightable Highlightable { get; private set; }
    public Selectable Selectable { get; private set; }

    public readonly List<PlaceableMachine> affectingMachines = new();

    Vector3 _originalPosition;
    Vector3 _introStartingPosition;

    [Header("Settings")]
    [SerializeField] public Biome Biome;
    [SerializeField] Vector2 _cordsOffset;
    [SerializeField] [Range(-25, -1)] float _introDepth;
    [SerializeField] [Range(1, 20)] float _introSpeed;

    public GroundTile[] neighbours;
    private void Awake()
    {
        Highlightable = GetComponent<Highlightable>();
        Selectable = GetComponent<Selectable>();
        Selectable.hoverAction += HighlightBiomeTiles;
        Selectable.stopHoverAction += UnhighlightBiomeTiles;
        Selectable.clickAction += PrintBiomeData;
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

    public List<MachineType> GetAffectingMachineTypes() 
    {
        List <MachineType> machineTypes = new List<MachineType>();  
        foreach(var machine in affectingMachines) 
        {
            if (!machineTypes.Contains(machine.GetMachineType())) machineTypes.Add(machine.GetMachineType());
        }

        return machineTypes;
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
        List<GroundTile> newNeighbours = new();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;
                if (groundGrid.TryGetValue(CellCoord + new Vector2Int(i, j), out GroundTile neighbour))
                {
                    newNeighbours.Add(neighbour);
                }
            }
        }

        this.neighbours = newNeighbours.ToArray();
        return newNeighbours;
    }

    public void SetBiome(Biome biome) 
    {
        Biome = biome;
        GetComponent<MeshFilter>().mesh = Biome.Mesh;
        GetComponent<MeshRenderer>().material = Biome.Material;
        foreach(GroundDecorationsCreator creator in GetComponentsInChildren<GroundDecorationsCreator>())
        {
            if(creator.Type == DecorationType.Grass)
            {
                creator.SetGrassColor(Biome.grassBottomColor, Biome.grassTopColor);
            }
        }
        Highlightable.UpdateOriginalMaterials();
        BiomeHandler.Instance.TilesPerBiome[Biome.Type].Add(this);
    }

    public void HighlightBiomeTiles() 
    {
        if (!Draggable.IsDragging) 
        {
            List < GroundTile > biomeTiles = BiomeHandler.Instance.TilesPerBiome[Biome.Type];
            foreach (GroundTile tile in biomeTiles) 
            {
                tile.Highlightable.Highlight("Highlight");
            }
        }
    }
    
    public void UnhighlightBiomeTiles() 
    {
        List < GroundTile > biomeTiles = BiomeHandler.Instance.TilesPerBiome[Biome.Type];
        foreach (GroundTile tile in biomeTiles) 
        {
            tile.Highlightable.Unhighlight();
        }
    }

    [ContextMenu("PrintTileData")]
    public void PrintTileData() 
    {
        string data;
        data = "Tile Data\n";
        data += "Coords: " + CellCoord.ToString() + "\n";
        data += "BiomeType: " + Biome.Type + "\n";

        Debug.Log(data);
    }

    [ContextMenu("PrintBiomeData")]
    public void PrintBiomeData() 
    {
        /*string data = "";
        data += "Biome Data\n";
        data += "Name: " + Biome.name + "\n";
        data += "Description: " + Biome.Description + "\n";
        data += "Type: " + Biome.Type + "\n";
        data += "spawnCount: " + Biome.spawnCount + "\n";
        data += "biomeWeight: " + Biome.biomeWeight + "\n";
        data += "tilePrefab.name: " + Biome.tilePrefab.name + "\n";
        Debug.Log(data);*/

        MachineShop.Instance.PopulateShop(Biome.Type);
        CompletionUIManager.Instance.ShowCompletionBar(Biome);
    }
}
