using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum LevelSceneFlow 
{
    Invalid = -1,
    PreLevel = 0,
    ShowBiomeBubbles,
    RelationPhase,
    Gameplay,
    OutsideGameplay = 99
}

public class ResourceGame : MonoBehaviour
{
    public static ResourceGame Instance;

    [SerializeField] public Level Level;

    public readonly List<Level> Levels = new();

    private LevelSceneFlow _currentSceneFlow;

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

    public LevelSceneFlow GetLevelSceneFlow() { return _currentSceneFlow; }

    public void ProcessActiveScene(LevelSceneFlow flow = LevelSceneFlow.Invalid)
    {
        switch (SceneLoader.GetActiveSceneIndex())
        {
            case SceneIndex.LEVEL_SCENE:
                _currentSceneFlow = flow;
                switch (flow) 
                {
                    case LevelSceneFlow.PreLevel:
                        Level.InitPreLevel();
                        break;
                    case LevelSceneFlow.ShowBiomeBubbles:
                        Level.InitBubblePhase();
                        break;
                    case LevelSceneFlow.RelationPhase:
                        Level.InitRelationLevel();
                        break;
                    case LevelSceneFlow.Gameplay:
                        Level.InitGameplayLevel();
                        break;
                    default:
                        Debug.LogError("Trying to load in game level flow with invalid state.");
                        break;
                }
                break;
            case SceneIndex.LEVEL_SELECTOR:
            case SceneIndex.MAIN_MENU:
                _currentSceneFlow = LevelSceneFlow.OutsideGameplay;
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

    public void UpdateLevelBubbleBiome(BiomeType biome) 
    {
        Level.CurrentRelationBiome = biome;
    }

    private void Update()
    {
        if (Debug.isDebugBuild) 
        {
            if (Input.GetKeyDown(KeyCode.W)) 
            {
                RelationHandler.Instance.KillBiomeBubbles();
                AnchorPoint.AllBiomesFinished.Invoke();
            }
        }
    }
}
