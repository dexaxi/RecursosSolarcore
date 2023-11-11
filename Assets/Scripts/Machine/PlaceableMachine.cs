using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Highlightable))]
[RequireComponent(typeof(Selectable))]
public class PlaceableMachine : Draggable
{
    [SerializeField] private Machine _machine;
    private Selectable _selectable;

    public override void OnMouseOver()
    {
        base.OnMouseOver();
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        _highlightable.Highlight("MouseEnter");
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        if (!_isDragging) _highlightable.Unhighlight();
    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public override void OnMouseUp() 
    {
        base.OnMouseUp();
        if (!_isMouseOver) _highlightable.Unhighlight();
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
    }

    public void ShowMachineInfo() 
    {
        if (IsDraggable) return;
        Debug.Log("ShowMachineInfo");
    }
}
