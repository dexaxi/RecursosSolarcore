using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Vector3 _posOffset;
    [SerializeField] private Vector3 _rotOffset;
    
    void Start()
    {
        CameraManager.Instance.UpdateCameraCanvas();
    }

    private void Update()
    {
        var camera = CameraManager.Instance.MainCamera;
        transform.LookAt(transform.position + camera.transform.rotation * -Vector3.back, camera.transform.rotation * Vector3.up);
        transform.position = transform.position + _posOffset;
        transform.Rotate(_rotOffset);
    }
}
