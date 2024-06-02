using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    //TEMP
    [field: SerializeField] private Button LoadLevel;

    private void Awake()
    {
        LoadLevel.onClick.AddListener(LoadAssignedLevel);
    }

    private void LoadAssignedLevel() 
    {
        ResourceGame.Instance.SetLevel("TestLevel");
        SceneLoader.Instance.LoadScene(SceneIndex.PROBLEM_SCREEN);
    }
}
