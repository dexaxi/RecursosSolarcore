using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private WaitForSecondsRealtime waitForFrequency;


    private void Awake()
    {
        if (Debug.isDebugBuild)
        {
            QualitySettings.vSyncCount = 0;
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            waitForFrequency = new WaitForSecondsRealtime(0.5f);
            StartCoroutine(FPS());
        }
        else Destroy(gameObject);
    }

    private IEnumerator FPS()
    {
        int lastFrameCount;
        float lastTime;
        float timeSpan;
        int frameCount;
        for (; ; )
        {
            // Capture frame-per-second
            lastFrameCount = Time.frameCount;
            lastTime = Time.realtimeSinceStartup;
            yield return waitForFrequency;
            timeSpan = Time.realtimeSinceStartup - lastTime;
            frameCount = Time.frameCount - lastFrameCount;
            float fps = Mathf.RoundToInt(frameCount / timeSpan);
            if (fps >= 60) _textMeshProUGUI.color = Color.green;
            else if (fps >= 30) _textMeshProUGUI.color = Color.yellow;
            else _textMeshProUGUI.color = Color.red;
            _textMeshProUGUI.text = string.Format("FPS: {0}", fps);
        }
    }
}
