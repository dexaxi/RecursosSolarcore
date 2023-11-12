using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableMachine : Draggable
{
    [Header("Machine Settings")]
    [SerializeField] private Machine _machine;

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
        _highlightable.Highlight("MouseEnter");
    }
    private void UnhighlightOnLeave() 
    {
        if (!_isDragging) _highlightable.Unhighlight();
    }

    public void ShowMachineInfo() 
    {
        if (IsDraggable) return;
        Debug.Log("ShowMachineInfo");
    }
}
