using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{

    public static void Scale(this Transform transform, float scale) 
    {
        transform.localScale = new Vector3(transform.localScale.x * scale, transform.localScale.y * scale, transform.localScale.z * scale);
    }
}
