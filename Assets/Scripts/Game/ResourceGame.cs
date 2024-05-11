using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGame : MonoBehaviour
{
    public static ResourceGame Instance;

    [SerializeField] public Level Level;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    void Start()
    {
        Level.InitLevel();
    }

}
