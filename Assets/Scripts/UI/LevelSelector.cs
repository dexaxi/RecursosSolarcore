using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    //TEMP
    [field: SerializeField] private List<Button> LevelButtons;

    private void Awake()
    {
        foreach(Button level in LevelButtons) 
        {
            LevelProvider levelProvider = level.GetComponent<LevelProvider>();
            level.onClick.AddListener( delegate { LoadAssignedLevel(levelProvider.LevelName); } );
        }
    }

    private void LoadAssignedLevel(string levelName) 
    {
        ResourceGame.Instance.SetLevel(levelName);
        SceneLoader.Instance.LoadScene(SceneIndex.LEVEL_SCENE, 300);
    }
}
