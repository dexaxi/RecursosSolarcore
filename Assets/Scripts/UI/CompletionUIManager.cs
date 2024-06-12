using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletionUIManager : MonoBehaviour
{
    public static CompletionUIManager Instance;
    
    [field: SerializeField] public List<CanvasGroup> PhaseUI { get; private set; } = new();
    [field: SerializeField] public List<Image> ProgressImages { get; private set; } = new();
    [field: SerializeField] public List<Scrollbar> ProgressScrolls { get; private set; } = new();
    [field: SerializeField] public List<TextMeshProUGUI> ProgressText { get; private set; } = new();
    [field: SerializeField] public List<Image> CheckIcons { get; private set; }
    [field: SerializeField] public Image Icon { get; private set; }
    [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }

    public void Start()
    {
        HideCompletionBar();
        CanvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowCompletionBar(Biome biome)
    {
        CanvasGroup.alpha = 1.0f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        UpdateUI(biome);
    }

    public void HideCompletionBar()
    {
        CanvasGroup.alpha = 0.0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }

    public void UpdateUI(Biome biome)
    {
        Icon.sprite = biome.Sprite;
        var problems = BiomePhaseHandler.Instance.ProblemsPerBiome[biome.Type];
        for (int i = 0; i < problems.Count; i++)
        {
            PhaseUI[i].alpha = 1.0f;
            PhaseUI[i].interactable = true;
            PhaseUI[i].blocksRaycasts = true;
            var currentCompletion = BiomePhaseHandler.Instance.CurrentCompletion[problems[i]];
            ProgressImages[i].fillAmount = (currentCompletion == 0 ? 0.0f : currentCompletion / 100.0f); ;
            int maxCompletion = (int) BiomePhaseHandler.Instance.MaxCompletion[problems[i]];
            ProgressScrolls[i].value = (float) maxCompletion / 100;
            if (currentCompletion >= maxCompletion) CheckIcons[i].enabled = true;
            else CheckIcons[i].enabled = false;
            ProgressScrolls[i].interactable = false;
            ProgressText[i].text = maxCompletion + "%";
        }

        for (int i = problems.Count; i < PhaseUI.Count; i++)
        {
            PhaseUI[i].alpha = 0.0f;
            PhaseUI[i].interactable = false;
            PhaseUI[i].blocksRaycasts = false;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


}
