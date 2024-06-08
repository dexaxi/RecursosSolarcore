using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Selectable : MonoBehaviour
{
    public static Selectable SELECTABLE_LOCK;
    Collider _collider;

    public Action clickAction;
    public Action selectAction;
    public Action deselectAction;
    public Action hoverAction;
    public Action stopHoverAction;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        if (gameObject.layer < LayerMask.NameToLayer("Selectable")) gameObject.layer = LayerMask.NameToLayer("Selectable");
    }

    public void Select()
    {
        if (IsUsingUI.IsUIEnabled()) return;
        if (SELECTABLE_LOCK != null && SELECTABLE_LOCK != this) return;
        selectAction?.Invoke();
    }

    public void Deselect()
    {
        if (IsUsingUI.IsUIEnabled()) return;
        if (SELECTABLE_LOCK != null && SELECTABLE_LOCK != this) return;
        deselectAction?.Invoke();
    }

    public void Click()
    {
        if (IsUsingUI.IsUIEnabled()) return;
        if (SELECTABLE_LOCK != null && SELECTABLE_LOCK != this) return;
        clickAction?.Invoke();
    }

    public void Hover() 
    {
        if (IsUsingUI.IsUIEnabled()) return;
        if (SELECTABLE_LOCK != null && SELECTABLE_LOCK != this) return;
        hoverAction?.Invoke();
    }

    public void StopHover() 
    {
        if (IsUsingUI.IsUIEnabled()) return;
        if (SELECTABLE_LOCK != null && SELECTABLE_LOCK != this) return;
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

    public static void UnlockSelectable() 
    {
        SELECTABLE_LOCK = null;
    }
}
