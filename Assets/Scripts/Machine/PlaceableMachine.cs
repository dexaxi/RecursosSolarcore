using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class PlaceableMachine : Draggable
{
    [Header("Machine Settings")]
    [SerializeField] private Machine _machine;
    [SerializeField] [Range(0, 1000000)] private float _machineReturnSpeed;

    [HideInInspector] public bool IsPlaced;
    public bool HasBeenFirstPlaced { get; private set; }
    public GroundTile GroundTile { get; private set; }

    public override void OnMouseOver()
    {
        base.OnMouseOver();
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
    }

    public override void OnMouseDown()
    {
        if (!IsPlaced) base.OnMouseDown();
    }

    public override void OnMouseUp()
    {
        base.OnMouseUp();
        UpdateGroundTile();
        HandleInvalidTerrainHighlight();
    }

    public override void OnMouseDrag()
    {
        if (IsPlaced) return;
        base.OnMouseDrag();
    }

    public override void Awake()
    {
        base.Awake();
        _machine = ScriptableObject.CreateInstance<Machine>();
    }

    public void Initialize()
    {
        HasBeenFirstPlaced = false;
        
        HandleHighlightableAwake();
        
        _selectable = GetComponent<Selectable>();
        HandleSelectActions();

        FindEmptyCells();

        MachineDisplay.Instance.SetMachine(this);
        MachineDisplay.Instance.ShowPlaceDisplay();
        
        UpdateGroundTile();
        HandleInvalidTerrainHighlight();
    }

    private void HandleHighlightableAwake() 
    {
        _highlightable = GetComponent<Highlightable>();
        if (_machine.MeshRenderer != null) GetComponent<MeshRenderer>().material = _machine.MeshRenderer;
        if (_machine.MeshFilter != null) GetComponent<MeshFilter>().mesh = _machine.MeshFilter;
        _highlightable.UpdateOriginalMaterials();
    }

    private void FindEmptyCells() 
    {
        Vector2Int cellCoords = Ground.Instance.FindEmptyCellCoords();
        transform.position = new Vector3(cellCoords.x, GetFixedHeight(), cellCoords.y);
    }

    private void HandleSelectActions() 
    {
        _selectable.clickAction += ShowMachineInfo;
        _selectable.hoverAction += HighlightOnHover;
        _selectable.stopHoverAction += UnhighlightOnLeave;
    }
    public void ShowMachineInfo()
    {
        if (!IsPlaced) return;
        DisplayMachineContextMenu();
    }

    private void HighlightOnHover() 
    {
        if(IsOnValidTerrain()) _highlightable.Highlight("MouseEnter");
    }
    private void UnhighlightOnLeave() 
    {
        if (!_isDragging && IsOnValidTerrain()) _highlightable.Unhighlight();
    }

    public void UpdateGroundTile()
    {
        Vector2Int cellCoords = Ground.Instance.ToCellCoords(transform.position);
        if (Ground.Instance.GroundMap.TryGetValue(cellCoords, out GroundTile tile))
        {
            if (GroundTile != null) GroundTile.isClosed = false;
            GroundTile = tile;
            GroundTile.isClosed = true;
        }
    }

    public void HandleInvalidTerrainHighlight()
    {
        if (!IsOnValidTerrain())
        {
            _highlightable.Highlight("InvalidTerrain");
        }
        else if (IsMouseOver)
        {
            _highlightable.Highlight("MouseEnter");
        }
    }

    public bool ConfirmPlacement()
    {
        IsPlaced = IsOnValidTerrain();
        HasBeenFirstPlaced = HasBeenFirstPlaced || IsOnValidTerrain();
        return IsOnValidTerrain();
    }
    
    public void Move()
    {
        IsPlaced = false;
    }
    
    public Vector2Int GetCoords() 
    {
        return Ground.Instance.ToCellCoords(transform.position);
    }

    public void Sell() 
    {
        PlayerCurrencyManager.Instance.AddCurrency(_machine.CalculateSellCost());
        Destroy(gameObject);
    }

    public void SellFullCost() 
    {
        PlayerCurrencyManager.Instance.AddCurrency(_machine.Cost);
        Destroy(gameObject);
    }

    private void DisplayMachineContextMenu() 
    {
        MachineDisplay.Instance.SetMachine(this);
        MachineDisplay.Instance.ShowSellDisplay();
    }

    public void CancelMove(Vector2Int prevCoords) 
    {
        StartCoroutine(MoveToPrevCoords(Ground.Instance.ToWorldCoords(prevCoords, GetFixedHeight())));
    }

    private IEnumerator MoveToPrevCoords(Vector3 target)
    {
        IsDragging = true;
        while (Vector3.Distance(transform.position, target) > 0.001f)
        {
            float step = _machineReturnSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            yield return new WaitForSeconds(0.01f);
        }
        IsDragging = false;
        UpdateGroundTile();
        UnhighlightOnLeave();
        MachineDisplay.Instance.ExitDisplay();
        IsPlaced = true;
    }

    public float GetFixedHeight() { return _snapToGrid.FixedHeight; }
    public bool IsOnValidTerrain()
    {
        bool validBiome = GroundTile?.Biome.Type != BiomeType.Water && GroundTile?.Biome.Type != BiomeType.Ocean;
        Vector2Int cellCoords = Ground.Instance.ToCellCoords(transform.position);
        bool hasValidCellCoords = cellCoords.x < Ground.Instance.MaxX && cellCoords.y < Ground.Instance.MaxY && cellCoords.x >= 0 && cellCoords.y >= 0;
        return validBiome && hasValidCellCoords;
    }

    public void CopyMachine(Machine machine) 
    {
        _machine.name = machine.name;
        _machine.Type = machine.Type;
        _machine.Description = machine.Description;
        _machine.Cost = machine.Cost;
        _machine.MeshFilter = machine.MeshFilter;
        _machine.MeshRenderer = machine.MeshRenderer;
        _machine.ShopSprite = machine.ShopSprite;
    }
}
