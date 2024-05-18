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

    public virtual void OnMouseOver() { }
    public virtual void OnMouseEnter()
    {
        _selectable.Hover();
        IsMouseOver = true;
    }
    public virtual void OnMouseExit()
    {
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
