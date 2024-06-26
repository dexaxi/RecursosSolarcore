using Cysharp.Threading.Tasks;
using DUJAL.Systems.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlaceableMachine : Draggable
{
    [Header("Machine Settings")]
    [SerializeField] private Machine _machine;
    [SerializeField][Range(0, 1000000)] private float _machineReturnSpeed;

    [HideInInspector] public bool IsPlaced;
    [SerializeField] public GameObject TimerGameObject;
    public bool HasBeenFirstPlaced { get; private set; }
    public GroundTile GroundTile { get; private set; }

    public Vector2Int PrevCoords { get; private set; }

    private Vector2Int _previousRangeCenterCoords;
    private List<GroundTile> _highlightedTiles;

    public int CurrentAffectingPhasePercent = 0;

    public float MachineProgress {  get; private set; }

    public override void OnMouseOver()
    {
        if (HandleSelectLockOverride()) _selectable.hoverAction.Invoke();
        base.OnMouseOver();
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
    }

    public override void OnMouseExit()
    {
        if (HandleSelectLockOverride()) _selectable.stopHoverAction.Invoke();
        base.OnMouseExit();
    }

    public override void OnMouseDown()
    {
        if (HandleSelectLockOverride())
        {
            _selectable.clickAction.Invoke();
        };
        if (!IsPlaced) base.OnMouseDown();
    }

    private bool HandleSelectLockOverride() 
    {
        // Override selectable lock if Placed && is another machine so we can switch between machine displays
        bool overrideLock = Selectable.SELECTABLE_LOCK != null
            && Selectable.SELECTABLE_LOCK.gameObject.GetComponent<PlaceableMachine>() != null
            && Selectable.SELECTABLE_LOCK.gameObject.GetComponent<PlaceableMachine>().IsPlaced;

        if (overrideLock && IsPlaced)
        {
            Selectable.SELECTABLE_LOCK = _selectable;
        }

        return overrideLock;
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
        UpdateRangeDisplay();
        if (IsMouseOver) _highlightable.Highlight("MouseEnter");
    }

    public override void Awake()
    {
        base.Awake();
        _machine = ScriptableObject.CreateInstance<Machine>();
        _highlightedTiles = new List<GroundTile>();
        MachineProgress = 0;
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

        Selectable.SELECTABLE_LOCK = GetComponent<Selectable>();
    }

    public MachineType GetMachineType() { return _machine.Type; }
    public Machine GetRelatedMachine() { return _machine; }

    private void HandleHighlightableAwake()
    {
        _highlightable = GetComponent<Highlightable>();
        if (_machine.MeshRenderer != null) GetComponent<MeshRenderer>().material = _machine.MeshRenderer;
        if (_machine.MeshFilter != null) GetComponent<MeshFilter>().mesh = _machine.MeshFilter;
        _highlightable.UpdateOriginalMaterials();
    }

    private void FindEmptyCells()
    {
        Vector2Int cellCoords = MachineHandler.Instance.FindEmptyCellCoords();
        transform.position = Ground.Instance.ToWorldCoords(cellCoords, GetFixedHeight());
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
        Selectable.SELECTABLE_LOCK = GetComponent<Selectable>();
        DisplayMachineContextMenu();
        UpdateRangeDisplay(true);
    }

    private void HighlightOnHover()
    {
        if (IsOnValidTerrain()) _highlightable.Highlight("MouseEnter");
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
            GroundTile = tile;
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
        bool isOnValidTerrain = IsOnValidTerrain();
        if (!isOnValidTerrain && !HasBeenFirstPlaced)
        {
            return false;
        }
        else if (!isOnValidTerrain)
        {
            CancelMove(PrevCoords);
        }
        else
        {
            MachineHandler.Instance.PlacedMachines.Remove(PrevCoords);
            MachineHandler.Instance.PlacedMachines[GetCoords()] = this;
        }
        HasBeenFirstPlaced = true;
        IsPlaced = true;
        UpdateRangeDisplay();
        _highlightable.Unhighlight();
        ApplyMachinePlacement();
        UpdateMachinePos(GroundTile.Biome.Type);
        BiomeHandler.Instance.PlayBiomeAudio(GroundTile.Biome.Type);
        return isOnValidTerrain;
    }

    public void UpdateMachinePos(BiomeType biome) 
    {
        if (BiomeSortingRule.IsWaterBiome(biome))
        {
            GetComponent<SnapToGrid>().FixedHeight -= 0.3f;
        }
    }

    public void Move()
    {
        IsPlaced = false;
        UpdateRangeDisplay(true);
        PrevCoords = GetCoords();
    }

    public Vector2Int GetCoords()
    {
        return Ground.Instance.ToCellCoords(transform.position);
    }

    public void Sell()
    {
        PlayerCurrencyManager.Instance.AddCurrency(_machine.CalculateSellCost());
        MachineHandler.Instance.PlacedMachines.Remove(GetCoords());
        UnHighlightRange();
        var problems = BiomePhaseHandler.Instance.ProblemsPerBiome[GroundTile.Biome.Type];
        foreach (var problem in problems) 
        {
            if (RelationHandler.Instance.GetProblem(problem).PossibleSolutions.Contains(_machine.Type)) 
            {
                var currentCompletion = BiomePhaseHandler.Instance.CurrentCompletion[problem];
                BiomePhaseHandler.Instance.CurrentCompletion[problem] = Mathf.Max(currentCompletion - CurrentAffectingPhasePercent, 0);
            }
        }
        BiomePhaseHandler.Instance.MachinePlaceRestrictionCount[_machine.Type]--;
        CompletionUIManager.Instance.UpdateUI(GroundTile.Biome);
        AudioManager.Instance.Play("Coin");
        Destroy(gameObject);
    }

    public void SellFullCost()
    {
        PlayerCurrencyManager.Instance.AddCurrency(_machine.Cost);
        MachineHandler.Instance.PlacedMachines.Remove(GetCoords());
        UnHighlightRange();
        Destroy(gameObject);
    }

    private void DisplayMachineContextMenu()
    {
        MachineDisplay.Instance.SetMachine(this);
        MachineDisplay.Instance.ShowSellDisplay();
    }

    public void CancelMove(Vector2Int prevCoords)
    {
        UnHighlightRange();
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
        IsPlaced = true;
        MachineDisplay.Instance.ShowSellDisplay();
        UpdateRangeDisplay(true);
    }

    public float GetFixedHeight() { return _snapToGrid.FixedHeight; }
    public bool IsOnValidTerrain()
    {
        GroundTile currentTile = Ground.Instance.GetTileFromCellCoords(GetCoords());
        bool validBiome = currentTile != null && IsBiomeCompatible(currentTile.Biome.Type);
        MachineHandler.Instance.PlacedMachines.TryGetValue(Ground.Instance.ToCellCoords(transform.position), out PlaceableMachine placeableMachine);
        bool isTileOccupiedByOtherMachine = placeableMachine != this && placeableMachine != null;
        return validBiome && !isTileOccupiedByOtherMachine;
    }

    public void CopyMachine(Machine machine)
    {
        _machine.Copy(machine);
    }

    public List<GroundTile> GetCurrentAffectedTiles() 
    {
        List<GroundTile> returnList = new();

        Vector2Int currentCoords = GetCoords();
        int[,] pattern = _machine.GetRangePattern();
        if (pattern == null) return returnList;
        Vector2Int centerCoords = new(pattern.GetLength(0) / 2, pattern.GetLength(1) / 2);

        int currX, currY;
        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            currX = currentCoords.x - centerCoords.x + i;
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                currY = currentCoords.y - centerCoords.y + j;
                GroundTile currentTile = Ground.Instance.GetTileFromCellCoords(new Vector2Int(currX, currY));
                if (currentTile == null) continue;
                if (pattern[i, j] == 1 && IsBiomeCompatible(currentTile.Biome.Type)) returnList.Add(currentTile);
            }
        }

        return returnList;
    }

    public void UpdateRangeDisplay(bool forced = false)
    {
        if (_machine.PatternType == PatternType.Biome) 
        {
            UpdateGroundTile();
            HighlightBiomeTiles();
            return;
        }
        bool showHighlight = !IsPlaced || forced;

        Vector2Int currentCoords = GetCoords();
        if (_previousRangeCenterCoords == currentCoords && showHighlight && !forced) return;
        _previousRangeCenterCoords = currentCoords;

        int[,] pattern = _machine.GetRangePattern();
        Vector2Int centerCoords = new(pattern.GetLength(0) / 2, pattern.GetLength(1) / 2);

        Queue<GroundTile> highlightedTiles = new Queue<GroundTile>();
        int currX, currY;
        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            currX = currentCoords.x - centerCoords.x + i;
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                currY = currentCoords.y - centerCoords.y + j;
                GroundTile currentTile = Ground.Instance.GetTileFromCellCoords(new Vector2Int(currX, currY));
                if (currentTile == null) continue;
                if (pattern[i, j] == 1) highlightedTiles.Enqueue(currentTile);
                else 
                {
                    currentTile.Highlightable.Unhighlight();
                }
            }
        }

        while (highlightedTiles.Count > 0)
        {
            GroundTile tile = highlightedTiles.Dequeue();
            string highlightMaterial = IsBiomeCompatible(tile.Biome.Type) ? "Highlight" : "InvalidBiome";
            if (showHighlight)
            {
                tile.Highlightable.Highlight(highlightMaterial);
                _highlightedTiles.Add(tile);
            }
            else
            {
                tile.Highlightable.Unhighlight();
                _highlightedTiles.Remove(tile);
            }
        }
    }

    private void HighlightBiomeTiles() 
    {
        var biomes = BiomeHandler.Instance.GetFilteredBiomes();
        foreach (Biome biome in biomes) 
        {
            List<GroundTile> biomeTiles = BiomeHandler.Instance.TilesPerBiome[biome.Type];
            foreach (GroundTile tile in biomeTiles)
            {
                if (tile.Biome.Type == GroundTile.Biome.Type)
                {
                    string highlightMaterial = IsBiomeCompatible(tile.Biome.Type) ? "Highlight" : "InvalidBiome";
                    tile.Highlightable.Highlight(highlightMaterial);
                    _highlightedTiles.Add(tile);
                }
                else
                {
                    tile.Highlightable.Unhighlight();
                    _highlightedTiles.Remove(tile);
                }
            }
        }
    }

    public void UnHighlightRange() 
    {
        foreach(GroundTile tile in _highlightedTiles)
        {
            tile.Highlightable.Unhighlight();
        }
        _highlightedTiles.Clear();
    }

    private bool IsBiomeCompatible(BiomeType biomeType)
    {
        return _machine.CompatibleBiomes.Contains(biomeType);
    }
    

    public int GetMachineWaiting() 
    {
        if (_machine.PatternType == PatternType.Biome) 
        {
            return 1600;
        }
        if (_machine.PatternType == PatternType.Pattern) 
        {
            return 1000;
        }
        return -1;
    }

    public void ApplyMachinePlacement() 
    {
        var progress = Instantiate(TimerGameObject).GetComponent<MachineProgressDisplay>();
        progress.MachineRef = this;
        if (_machine.PatternType == PatternType.Biome)
        {
            AudioManager.Instance.Play("machine_long");
        }
        if (_machine.PatternType == PatternType.Pattern)
        {
            AudioManager.Instance.Play("machine_short");
        }

        MachinePlacementTimer().Forget();
    }

    public async UniTask MachinePlacementTimer() 
    {
        await UniTask.Delay(GetMachineWaiting() / 100);
        MachineProgress += 0.01f;
        MachinePlacementTimer().Forget();
    }

    /// <summary>
    ///  DEBUG
    /// </summary>

    [ContextMenu("PrintMachineRange")]
    public void PrintMachineRange() 
    {
        string printableString = "";
        int[,] pattern = _machine.GetRangePattern();
        for (int i = 0; i < pattern.GetLength(0); i++) 
        {
            for (int j = 0; j < pattern.GetLength(1); j++) 
            {
                printableString += pattern[i, j].ToString();
            }
            printableString += "\n";
        }
        Debug.Log(printableString);
    }
}
