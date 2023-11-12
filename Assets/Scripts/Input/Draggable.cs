using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Draggable : Hoverable
{
    public static bool IsDragging;

    public bool IsDraggable { get; private set; } = true;

    protected Vector3 _mousePosOffset;
    protected bool _isDragging;

    public virtual void OnMouseDown()
    {
        Vector3 offsetScreenPos = new(Input.mousePosition.x, Input.mousePosition.y, GetDraggableItemWorldPosToScreenPoint().z);
        _mousePosOffset = transform.position - CameraUtils.MainCamera.ScreenToWorldPoint(offsetScreenPos);
    }

    public virtual void OnMouseUp() 
    {
        _isDragging = false;
        IsDragging = false;
    }

    public virtual void OnMouseDrag()
    {
        _isDragging = true;
        IsDragging = true;
        if (IsDraggable) 
        {
            Vector3 screenPos = new(Input.mousePosition.x, Input.mousePosition.y, GetDraggableItemWorldPosToScreenPoint().z);
            transform.position = CameraUtils.MainCamera.ScreenToWorldPoint(screenPos) + _mousePosOffset;
        }
    }

    public void SetDraggable(bool isDraggable) 
    {
        IsDraggable = isDraggable;
    }

    private Vector3 GetDraggableItemWorldPosToScreenPoint() 
    {
        return CameraUtils.MainCamera.WorldToScreenPoint(transform.position);
    }

    public virtual void Awake()
    {
        if (gameObject.layer < LayerMask.NameToLayer("Draggable")) gameObject.layer = LayerMask.NameToLayer("Draggable");
    }
}
