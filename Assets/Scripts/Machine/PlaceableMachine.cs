using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class PlaceableMachine : Draggable
{
    [Header("Machine Settings")]
    [SerializeField] private Machine _machine;

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
        base.OnMouseDown();
    }

    public override void OnMouseUp()
    {
        base.OnMouseUp();
        if (Ground.Instance.GroundMap.TryGetValue(Ground.Instance.ToCellCoords(transform.position), out GroundTile tile))
        {
            GroundTile = tile;
        }

        if (!IsOnValidTerrain())
        {
            _highlightable.Highlight("InvalidTerrain");
        }
        else if (IsMouseOver)
        {
            _highlightable.Highlight("MouseEnter");
        }
    }

    public override void OnMouseDrag()
    {
        base.OnMouseDrag();
    }

    public override void Awake()
    {
        base.Awake();
        _highlightable = GetComponent<Highlightable>();

        _selectable = GetComponent<Selectable>();
        HandleSelectActions();
    }

    private void HandleSelectActions() 
    {
        _selectable.clickAction += ShowMachineInfo;
        _selectable.hoverAction += HighlightOnHover;
        _selectable.stopHoverAction += UnhighlightOnLeave;
    }

    private void HighlightOnHover() 
    {
        if(IsOnValidTerrain()) _highlightable.Highlight("MouseEnter");
    }
    private void UnhighlightOnLeave() 
    {
        if (!_isDragging && IsOnValidTerrain()) _highlightable.Unhighlight();
    }

    public void ShowMachineInfo() 
    {
        if (IsDraggable) return;
        Debug.Log("ShowMachineInfo");
    }

    public bool IsOnValidTerrain() 
    {
        return GroundTile?.Biome.Type != BiomeType.Water;
    }
}
