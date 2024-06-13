using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private Button openSettings;
	[SerializeField] private Button closeSettings;
	[SerializeField] private Button exit;
	[SerializeField] private Button reload;

	public bool StopDeltaTime;

	private void Awake()
	{
		openSettings.onClick.AddListener(OpenSettings);
		closeSettings.onClick.AddListener(CloseSettings);
		exit.onClick.AddListener(Exit);
		reload.onClick.AddListener(Reload);

		CloseSettings();
	}

	private void OpenSettings()
	{
		if(StopDeltaTime)
		{
			Time.timeScale = 0;
		}
		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;
	}

	private void CloseSettings()
	{
		if(StopDeltaTime)
		{
			Time.timeScale = 1;
		}
		canvasGroup.alpha = 0;
		canvasGroup.blocksRaycasts = false;
	}

	private void Exit()
	{
		SceneLoader.Instance.LoadScene(SceneIndex.MAIN_MENU);
	}

	private void Reload()
	{
		SceneLoader.Instance.ReloadScene();
	}
}