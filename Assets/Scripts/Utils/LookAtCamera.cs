using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    void Start()
    {
        CameraManager.Instance.UpdateCameraCanvas();
        transform.forward = CameraManager.Instance.transform.forward;   
        transform.Rotate(_offset);
    }
}
