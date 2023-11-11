using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Header("Fixed Height")]
    [SerializeField] bool _doFixHeight = true;
    [SerializeField] float _fixedHeight = 0;

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
            position.y = _fixedHeight;

        if (ZcellSize != 0)
            position.z = Mathf.RoundToInt(position.z / ZcellSize) * ZcellSize;

        if (_localPosition)
            transform.localPosition = position;
        else
            transform.position = position;
    }
}

