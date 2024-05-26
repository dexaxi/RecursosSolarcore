using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Highlightable))]
public class Hoverable : MonoBehaviour
{
    protected Highlightable _highlightable;
    protected Selectable _selectable;

    public bool IsMouseOver { get; private set; }

    public virtual void OnMouseOver() 
    {
        if (IsUsingUI.IsUIEnabled()) return;
        if (Selectable.SELECTABLE_LOCK != null && Selectable.SELECTABLE_LOCK != _selectable) return;
    }

    public virtual void OnMouseEnter()
    {
        if (IsUsingUI.IsUIEnabled()) return;
        if (Selectable.SELECTABLE_LOCK != null && Selectable.SELECTABLE_LOCK != _selectable) return;

        _selectable.Hover();
        IsMouseOver = true;
    }
    public virtual void OnMouseExit()
    {
        if (IsUsingUI.IsUIEnabled()) return;
        if (Selectable.SELECTABLE_LOCK != null && Selectable.SELECTABLE_LOCK != _selectable) return;

        _selectable.StopHover();
        IsMouseOver = false;
    }
    public virtual void Awake()
    {
        if (_highlightable == null) _highlightable = GetComponent<Highlightable>();
        if (_selectable == null) _selectable = GetComponent<Selectable>();
    }

    public virtual void Start() 
    {
        
    }
}
