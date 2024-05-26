using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    //TEMP
    [field: SerializeField] private Button Level1Button;

    private void Awake()
    {
        Level1Button.onClick.AddListener(LoadLevel1);
    }

    private void LoadLevel1() 
    {
        SceneLoader.Instance.LoadScene(SceneIndex.LEVEL_SCENE);
    }
}
