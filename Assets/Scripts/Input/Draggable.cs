using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Draggable : Hoverable
{
    public static bool IsDragging;

    public bool IsDraggable { get; private set; } = true;

    protected Vector3 _mousePosOffset;
    protected bool _isDragging;

    private Vector3 _prevScale;

    SnapToGrid _snapToGrid;
    float _prevHeight;

    [Header("Draggable Settings")]
    [SerializeField] protected bool doScaleOnDrag;
    [SerializeField] protected bool doHeightOnDrag;
    [SerializeField] protected float scaleOnDrag;
    [SerializeField] protected float heightOnDrag;

    public virtual void OnMouseDown()
    {
        Vector3 offsetScreenPos = new(Input.mousePosition.x, Input.mousePosition.y, GetDraggableItemWorldPosToScreenPoint().z);
        _mousePosOffset = transform.position - CameraUtils.MainCamera.ScreenToWorldPoint(offsetScreenPos);
        _prevScale = transform.localScale;
        if (doScaleOnDrag) transform.Scale(scaleOnDrag);
        if (_snapToGrid != null)
        {
            _prevHeight = _snapToGrid.FixedHeight;
            if(doHeightOnDrag) _snapToGrid.FixedHeight = _prevHeight + heightOnDrag;
        }
    }

    public virtual void OnMouseUp() 
    {
        //private
        _isDragging = false;
        //Static
        IsDragging = false;
        transform.localScale = _prevScale;
        if (_snapToGrid != null)
        {
            if(doHeightOnDrag) _snapToGrid.FixedHeight = _prevHeight;
        }
    }

    public virtual void OnMouseDrag()
    {
        //private
        _isDragging = true;
        //static
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
        _snapToGrid = GetComponent<SnapToGrid>();
    }
}
