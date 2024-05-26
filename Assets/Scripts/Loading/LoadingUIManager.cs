using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIManager : MonoBehaviour
{
    private Slider _progressBar;
    private TextMeshProUGUI _loadingText;

    private void Start()
    {
        _progressBar = GetComponentInChildren<Slider>();
        _loadingText = GetComponentInChildren<TextMeshProUGUI>();
        _progressBar.value = 0;
        _loadingText.text = "0%";
    }

    private void Update()
    {
        //Loading progress bar + % text
        if (SceneLoader.Instance.SceneLoadAsyncOperation != null && !SceneLoader.Instance.SceneLoadAsyncOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(SceneLoader.Instance.SceneLoadAsyncOperation.progress / 0.9f);
            _progressBar.value = progressValue;
            _loadingText.text = Mathf.Round(progressValue * 100) + "%";
        }
    }
}
