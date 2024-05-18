using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Selectable : MonoBehaviour
{
    Collider _collider;

    public Action clickAction;
    public Action selectAction;
    public Action deselectAction;
    public Action hoverAction;
    public Action stopHoverAction;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        if (gameObject.layer < LayerMask.NameToLayer("Selectable")) gameObject.layer = LayerMask.NameToLayer("Selectable");
    }

    public void Select()
    {
        if (IsUsingUI.IsUIEnabled()) return;
        selectAction?.Invoke();
    }

    public void Deselect()
    {
        if (IsUsingUI.IsUIEnabled()) return;
        deselectAction?.Invoke();
    }

    public void Click()
    {
        if (IsUsingUI.IsUIEnabled()) return;
        clickAction?.Invoke();
    }

    public void Hover() 
    {
        if (IsUsingUI.IsUIEnabled()) return;
        hoverAction?.Invoke();
    }

    public void StopHover() 
    {
        if (IsUsingUI.IsUIEnabled()) return;
        stopHoverAction?.Invoke(); 
    }

    public void EnableSelection()
    {
        _collider.enabled = true;
    }

    public void DisableSelection()
    {
        _collider.enabled = false;
    }
}
