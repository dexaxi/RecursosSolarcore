using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private float _fps;
    public void GetFPS() 
    {
        _fps = (int)(1f / Time.unscaledDeltaTime);
        
    }

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        InvokeRepeating("GetFPS", 0.2f, 0.5f);
    }

    private void Update()
    {
        _textMeshProUGUI.text = "FPS: " + _fps.ToString();
        if (_fps >= 60) _textMeshProUGUI.color = Color.green; 
        else if (_fps >= 30) _textMeshProUGUI.color = Color.yellow; 
        else _textMeshProUGUI.color = Color.red; 
    }
}
