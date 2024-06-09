using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ResourceGame : MonoBehaviour
{
    public static ResourceGame Instance;

    [SerializeField] public Level Level;

    public readonly List<Level> Levels = new();


    private void Awake()
    {
        if (Instance) 
        {
            Destroy(gameObject);
            return;
        }
        else 
        {
            Instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }

        PopulatetAllLevels();
    }
    
    public void ProcessActiveScene()
    {
        switch (SceneLoader.GetActiveSceneIndex())
        {
            case SceneIndex.LEVEL_SCENE:
                Level.InitLevel();
                break;
            case SceneIndex.PROBLEM_SCREEN:
                Level.InitRelationLevel();
                break;
            case SceneIndex.LEVEL_SELECTOR:
            case SceneIndex.MAIN_MENU:
                break;
            case SceneIndex.NO_SCENE:
            default:
                Debug.LogError($"TRYING TO OPEN INVALID SCENE: {SceneLoader.GetActiveSceneIndex()}");
                break;
        }
    }

    public void SetLevel(string level) 
    {
        Level = GetLevelFromString(level)
;
        Debug.Log($"[RESOURCE GAME]: ASSIGNED LEVEL {Level.name}");
    }

    private Level GetLevelFromString(string levelName) 
    {
        foreach (Level level in Levels) 
        {
            if (level.name == levelName) return level;
        }
        Debug.LogWarning($"WARNING: No scene with name: {levelName} found");
        return null;
    }

    public void PopulatetAllLevels() 
    {
        Levels.Clear();
        var levelArray = Resources.LoadAll("ScriptableObjects/Levels", typeof(Level));
        foreach (Level level in levelArray.Cast<Level>())
        {
            Levels.Add(level);
        }
    }

}
