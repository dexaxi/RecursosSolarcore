using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneIndex
{
    NO_SCENE = -1,
    MAIN_MENU = 0,
    LEVEL_SELECTOR = 1,
    LOADING_SCREEN = 2,
    PROBLEM_SCREEN = 3,
    LEVEL_SCENE = 4,
}

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [HideInInspector] public bool IS_LOADING { get; private set; }
    [HideInInspector] public bool HOLD_LOADING;

    private AsyncOperation _sceneLoadOp;
    private AsyncOperation _sceneUnloadOp;
    private AsyncOperation _loadingSceneOp;
    private AsyncOperation _unloadingSceneOp;

    private readonly Queue<SceneIndex> _loadingQueue = new();
    private SceneIndex _previousQueuedItem;

    private float _loadingProgress = 0;
    private bool _loadingSceneLoaded;
    private bool _sceneLoaded;
    private bool _sceneUnloaded;
    private bool _processingQueue;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else
        {
            Instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        HOLD_LOADING = false;
        ResetLoadingQueueControl();
        _previousQueuedItem = GetActiveSceneIndex();
    }

    //Public function to load scene from index 
    public void LoadScene(SceneIndex nextSceneIndex) 
    {
        if (_loadingQueue.Count == 0) _loadingProgress = 0;
        HOLD_LOADING = false;
        IS_LOADING = true;
        LoadSceneTask(nextSceneIndex).Forget();
    }

    private async UniTask LoadSceneTask(SceneIndex nextSceneIndex)
    {
        HandleLoadingSceneLoad().Forget();
        await UniTask.WaitUntil(() => _loadingSceneLoaded);
        _loadingQueue.Enqueue(nextSceneIndex);
        LoadSceneFlow().Forget();
    }

    public void LoadNextScene() { LoadScene(GetActiveSceneIndex() + 1); }

    public void LoadPreviousScene() { LoadScene(GetActiveSceneIndex() - 1); }

    //Coroutine
    public async UniTask LoadSceneFlow()
    {
        await UniTask.WaitUntil(() => !_processingQueue);
        
        _processingQueue = true;
        if (_loadingQueue.Count == 0) 
        {
            HandleLoadingQueueError(_previousQueuedItem);
            return;
        } 

        SceneIndex nextScreen = _loadingQueue.Dequeue();
        
        PrintUnloadingScene(_previousQueuedItem);

        HandleSceneUnload(_previousQueuedItem).Forget();
        
        await UniTask.WaitUntil(() => _sceneUnloaded);

        PrintUnloadingFinished(_previousQueuedItem);
        _previousQueuedItem = nextScreen;
        PrintLoadingScene(nextScreen);

        HandleSceneLoad(nextScreen).Forget();

        await UniTask.WaitUntil(() => _sceneLoaded);

        PrintLoadingFinished(nextScreen);
        ResetLoadingQueueControl();

        if (_loadingQueue.Count == 0)
        {
            HandleLoadEnd().Forget();
        }
    }

    private async UniTask HandleLoadEnd()
    {
        HandleLoadingSceneUnload().Forget();
        await UniTask.WaitUntil(() => !_loadingSceneLoaded);
        _sceneLoadOp = null;
        _sceneUnloadOp = null;
        _loadingSceneOp = null;
        _unloadingSceneOp = null;
        IS_LOADING = false;
        _loadingProgress = 100.0f;
    }

    private void HandleLoadingQueueError(SceneIndex previousLoadedScene) 
    {
        Debug.LogWarning($"[FINISHING LOAD] Warning: Trying to Dequeue Loading Sceen while queue is empty, last loaded item: {previousLoadedScene}. ");
        IS_LOADING = false;
        HOLD_LOADING = false;
        ResetLoadingQueueControl();
        HandleLoadingSceneUnload().Forget();
    }

    private void ResetLoadingQueueControl()
    {
        _sceneLoaded = false;
        _sceneUnloaded = false;
        _processingQueue = false;
    }

    private async UniTask HandleSceneLoad(SceneIndex scene) 
    {
        _sceneLoadOp = LoadAsync(scene, LoadSceneMode.Additive);

        //wait until loading is done
        await UniTask.WaitUntil(() => _sceneLoadOp.isDone
            && !HOLD_LOADING
            && IsSceneLoaded(scene));

        _sceneLoaded = true;
    }

    private async UniTask HandleSceneUnload(SceneIndex scene)
    {
        _sceneUnloadOp = UnloadAsync(scene);

        await UniTask.WaitUntil(() => _sceneUnloadOp.isDone
            && !IsSceneLoaded(_previousQueuedItem));

        _sceneUnloaded = true;
    }

    private async UniTask HandleLoadingSceneLoad() 
    {
        if (!IsSceneLoaded(SceneIndex.LOADING_SCREEN))
        {
            _loadingSceneOp = LoadAsync(SceneIndex.LOADING_SCREEN, LoadSceneMode.Additive);
        }
        await UniTask.WaitUntil(() => _loadingSceneOp.isDone
            && IsSceneLoaded(SceneIndex.LOADING_SCREEN));
        _loadingSceneLoaded = true;
    }

    private async UniTask HandleLoadingSceneUnload() 
    {
        if (IsSceneLoaded(SceneIndex.LOADING_SCREEN))
        {
            _unloadingSceneOp = UnloadAsync(SceneIndex.LOADING_SCREEN);
            await UniTask.WaitUntil(() => _unloadingSceneOp.isDone);
        }
        await UniTask.WaitUntil(() => !IsSceneLoaded(SceneIndex.LOADING_SCREEN));
        _loadingSceneLoaded = false;
    }

    public static bool IsSceneLoaded(SceneIndex sceneIndex) 
    {
        return SceneManager.GetSceneByBuildIndex((int)sceneIndex).isLoaded;
    }

    public static SceneIndex GetActiveSceneIndex() 
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if ((SceneIndex)scene.buildIndex != SceneIndex.LOADING_SCREEN)
            {
                return (SceneIndex) scene.buildIndex;
            }
        }

        Debug.LogWarning("WARNING: No active scene found");
        return SceneIndex.NO_SCENE;
    }

    private AsyncOperation LoadAsync(SceneIndex index, LoadSceneMode sceneMode) 
    {
        return SceneManager.LoadSceneAsync((int)index, sceneMode);
    }
    
    private AsyncOperation UnloadAsync(SceneIndex index) 
    {
        return SceneManager.UnloadSceneAsync((int)index);
    }

    public void ReloadScene()
    {
        LoadScene(GetActiveSceneIndex());
    }

    public string SceneIndexToString(SceneIndex index) 
    {
        return index switch
        {
            SceneIndex.NO_SCENE => "NO_SCENE",
            SceneIndex.MAIN_MENU => "MAIN_MENU",
            SceneIndex.LEVEL_SELECTOR => "LEVEL_SELECTOR",
            SceneIndex.LOADING_SCREEN => "LOADING_SCREEN",
            SceneIndex.PROBLEM_SCREEN => "PROBLEM_SCREEN",
            SceneIndex.LEVEL_SCENE => "LEVEL_SCENE",
            _ => "INVALID_SCENE",
        };
    }

    public float GetLoadingProgress() 
    {
        _loadingProgress = _sceneLoadOp != null && _loadingProgress < _sceneLoadOp.progress ? _sceneLoadOp.progress : _loadingProgress;
        return _loadingProgress;
    }

    public void PrintLoadingScene(SceneIndex index) 
    {
        Debug.Log("[LOADING] " + SceneIndexToString(index) );
    }
    
    public void PrintUnloadingScene(SceneIndex index) 
    {
        Debug.Log("[UNLOADING] " + SceneIndexToString(index) );
    }
    
    public void PrintLoadingFinished(SceneIndex index) 
    {
        Debug.Log("[LOADING] " + SceneIndexToString(index) + " FINISHED");
    }
    
    public void PrintUnloadingFinished(SceneIndex index) 
    {
        Debug.Log("[UNLOADING] " + SceneIndexToString(index) + " FINISHED");
    }
}
