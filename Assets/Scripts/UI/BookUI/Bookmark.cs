using DUJAL.Systems.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bookmark : MonoBehaviour
{
    [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
    [field: SerializeField] public Image Image { get; private set; }
    [field: SerializeField] public Image Icon { get; private set; }
    [field: SerializeField] public Button OpenBookButton { get; private set; }

    public BiomeType BiomeType { get; private set; }

    private void Start()
    {
        OpenBookButton.onClick.AddListener(UpdateBook);
        CanvasGroup.alpha = 0.0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }

    public void SetBiome(BiomeType biome) 
    {
        BiomeType = biome;
    }

    public void UpdateBook() 
    {
        RoboDialogueManager.Instance.SwitchToRelationDialogue();
        AudioManager.Instance.Play("Page_Turn");
        RelationHandler.Instance.InitBookUI(BiomeType);
    }
}
