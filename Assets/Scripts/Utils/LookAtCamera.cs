using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    void Start()
    {
        CameraManager.Instance.UpdateCameraCanvas();
        transform.LookAt(CameraManager.Instance.MainCamera.transform, Vector3.up);
        transform.Rotate(_offset);
    }
}
