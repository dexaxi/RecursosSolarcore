using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [field: SerializeField] private Button PlayButton;
    [field: SerializeField] private Button QuitButton;
    [field: SerializeField] private Button SettingsButton;
    [field: SerializeField] private Button ControlsButton;

    private void Awake()
    {
        PlayButton.onClick.AddListener(Play);
        QuitButton.onClick.AddListener(QuitGame);
        SettingsButton.onClick.AddListener(LoadSettings);
        ControlsButton.onClick.AddListener(LoadControls);
    }

    private void Play() 
    {
        SceneLoader.Instance.LoadScene(SceneIndex.LEVEL_SELECTOR);
    }
    private void QuitGame()
    {
        Application.Quit();
    }

    private void LoadSettings() 
    {
        Debug.Log("Settings!");
    }

    private void LoadControls() 
    {
        Debug.Log("Controls!");
    }

}
