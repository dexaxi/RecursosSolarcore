using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera MainCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        MainCamera = GetComponent<Camera>();
    }

    public void SetCameraTransform(Vector3 Position, Vector3 Rotation)
    {
        MainCamera.transform.SetPositionAndRotation(Position, Quaternion.Euler(Rotation));
    }

    public void SetBookCamera() 
    {
        SetCameraTransform(new Vector3(-20, 0, 0), Vector3.zero);
        MainCamera.orthographic = true;
        UpdateCameraCanvas(); 
    }
    public void SetGameplayCamera() 
    {
        SetCameraTransform(new Vector3(8, 11, -8), new Vector3(45, 0, 0));
        MainCamera.orthographic = false;
        UpdateCameraCanvas();
    }

    public void UpdateCameraCanvas() 
    {
        List<Canvas> canvases = FindObjectsOfType<Canvas>().ToList();
        foreach (Canvas c in canvases)
        {
            c.worldCamera = MainCamera;
        }
    }

}
