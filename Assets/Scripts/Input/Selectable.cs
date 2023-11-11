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

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        if (gameObject.layer < LayerMask.NameToLayer("Selectable")) gameObject.layer = LayerMask.NameToLayer("Selectable");
    }

    public void Select()
    {
        selectAction?.Invoke();
    }

    public void Deselect()
    {
        deselectAction?.Invoke();
    }

    public void Click()
    {
        clickAction?.Invoke();
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
