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
        transform.position = transform.position + _posOffset;
    }

    private void Update()
    {
        var camera = CameraManager.Instance.MainCamera;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        transform.Rotate(_rotOffset);
    }
}
