using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
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
    public AsyncOperation SceneLoadAsyncOperation;
    private AsyncOperation _loadingSceneOp;
    
    public bool IS_LOADING { get; private set; }
    public bool HOLD_LOADING;

    private Queue<SceneIndex> _loadingQueue = new Queue<SceneIndex>();
    private SceneIndex _previousQueuedItem;

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
        _previousQueuedItem = (SceneIndex) SceneManager.GetActiveScene().buildIndex;
    }

    //Public function to load scene from index 
    public void LoadScene(SceneIndex nextSceneIndex)
    {
        _loadingQueue.Enqueue(nextSceneIndex);
        StartCoroutine(LoadSceneAsync());
    }

    public void HandleQueue() 
    {
        StartCoroutine(LoadSceneAsync());
    }

    public void LoadNextScene() { LoadScene( (SceneIndex) (SceneManager.GetActiveScene().buildIndex + 1)); }

    public void LoadPreviousScene() { LoadScene( (SceneIndex) (SceneManager.GetActiveScene().buildIndex - 1) ); }

    //Coroutine
    public IEnumerator LoadSceneAsync()
    {
        IS_LOADING = true;
        SceneIndex nextScreen = _loadingQueue.Dequeue();
        
        PrintLoadingScene(nextScreen);
        PrintUnloadingScene(_previousQueuedItem);

        _loadingSceneOp = SceneManager.LoadSceneAsync( (int) SceneIndex.LOADING_SCREEN, LoadSceneMode.Single);
        
        while (!_loadingSceneOp.isDone)
        {
            yield return null;
        }

        PrintUnloadingFinished(_previousQueuedItem);
        _previousQueuedItem = nextScreen;
         

        SceneLoadAsyncOperation = SceneManager.LoadSceneAsync( (int) nextScreen, LoadSceneMode.Additive);

        //wait until loading is done
        while (HOLD_LOADING && !SceneLoadAsyncOperation.isDone)
        {
            yield return null;
        }
        
        PrintLoadingFinished(nextScreen);

        yield return new WaitForSeconds(1);

        if (_loadingQueue.Count == 0) 
        {
            var loadingUnload = SceneManager.UnloadSceneAsync((int) SceneIndex.LOADING_SCREEN );
            while (loadingUnload != null && !loadingUnload.isDone) 
            {
                yield return null;
            }
            IS_LOADING = false;
        }
        else HandleQueue();
    }

    public void ReloadScene()
    {
        LoadScene( (SceneIndex) SceneManager.GetActiveScene().buildIndex );
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

    public void PrintLoadingScene(SceneIndex index) 
    {
        Debug.Log("LOADING: " + SceneIndexToString(index) );
    }
    
    public void PrintUnloadingScene(SceneIndex index) 
    {
        Debug.Log("UNLOADING: " + SceneIndexToString(index) );
    }
    
    public void PrintLoadingFinished(SceneIndex index) 
    {
        Debug.Log("LOADING: " + SceneIndexToString(index) + " FINISHED");
    }
    
    public void PrintUnloadingFinished(SceneIndex index) 
    {
        Debug.Log("UNLOADING: " + SceneIndexToString(index) + " FINISHED");
    }
}
