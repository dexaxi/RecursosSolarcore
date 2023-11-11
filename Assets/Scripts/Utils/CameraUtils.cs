using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtils
{
    private static Camera _mainCamera;
    public static Camera MainCamera
    {
        get
        {
            if (!_mainCamera)
            {
                _mainCamera = Camera.main;
            }
            return _mainCamera;
        }
    }
}
