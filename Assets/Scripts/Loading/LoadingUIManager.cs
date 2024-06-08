using AnKuchen.Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIManager : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI _loadingText { get; private set; }
    
    private CanvasGroup _loadingCanvas;
    private Slider _progressBar;
    private void Start()
    {
        _progressBar = GetComponentInChildren<Slider>();
        _loadingCanvas = GetComponent<CanvasGroup>();
        if (_progressBar != null) _progressBar.value = 0;
        if(_loadingText != null) _loadingText.text = "0%";
    }

    private void Update()
    {
        //Loading progress bar + % text
        if (SceneLoader.Instance.IGNORE_LOADING_SCREEN) 
        {
            _loadingCanvas.alpha = 0.0f;
            return;
        }
        if (SceneLoader.Instance.IS_LOADING)
        {
            float progressValue = Mathf.Clamp01(SceneLoader.Instance.GetLoadingProgress());
            if(_progressBar != null) _progressBar.value = progressValue;
            if(_loadingText != null) _loadingText.text = Mathf.Round(progressValue * 100) + "%";
        }
    }
}


