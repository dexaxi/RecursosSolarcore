using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour
{
    const int _cellMaxSize = 100;
    [Space(3)]
    [Range(0.1f, _cellMaxSize)]
    public float XcellSize = 2f;

    [Space(3)]
    [Range(0.1f, _cellMaxSize)]
    public float YcellSize = 0.5f;

    [Space(3)]
    [Range(0.1f, _cellMaxSize)]
    public float ZcellSize = 2f;
    
    [Space(3)]
    [Header("Settings")]
    [SerializeField] bool _snapToHeight = true;
    [SerializeField] bool _localPosition;
    [SerializeField] bool _smooth;
    [SerializeField] [Range(0, 10)] float _smoothSpeed;
    [Header("Fixed Height")]
    [SerializeField] bool _doFixHeight = true;
    [SerializeField] public float FixedHeight = 0;

    private Vector3 _vel;
    private float _lerpSpeed;

    private void Update()
    {

        Vector3 position;

        if (_localPosition)
            position = transform.localPosition;
        else
            position = transform.position;

        if (XcellSize != 0)
            position.x = Mathf.RoundToInt(position.x / XcellSize) * XcellSize;

        if (_snapToHeight && YcellSize != 0 && !_doFixHeight)
            position.y = Mathf.RoundToInt(position.y / YcellSize) * YcellSize;
        else
            position.y = FixedHeight;

        if (ZcellSize != 0)
            position.z = Mathf.RoundToInt(position.z / ZcellSize) * ZcellSize;

        Vector3 sourcePosition;
        Vector3 targetPosition;

        if (_localPosition)
            sourcePosition = transform.localPosition;
        else
            sourcePosition = transform.position;

        if (_doFixHeight) sourcePosition.y = FixedHeight;

        if (!_smooth) _lerpSpeed = 0;
        else _lerpSpeed = _smoothSpeed;
        if (Draggable.IsDragging) _lerpSpeed = 10000000;
        targetPosition = Vector3.SmoothDamp(sourcePosition, position, ref _vel, Time.deltaTime * _lerpSpeed);
        if (_localPosition)
            transform.localPosition = targetPosition;
        else
            transform.position = targetPosition;

    }

    public Vector3 GetSnapVelocity() 
    {
        return _vel;
    }
}

