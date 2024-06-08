using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    [SerializeField] GameObject RenderedLine;

    public LineRenderer SpawnLine(Vector3 pos, AnchorPoint parent) 
    {
        var go = Instantiate(RenderedLine, pos, Quaternion.identity, parent.transform);
        LineRenderer line = go?.GetComponent<LineRenderer>();
        if (line != null) { line.positionCount = 0; }
        return line;
    }
}
