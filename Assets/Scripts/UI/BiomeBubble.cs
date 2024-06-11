using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class BiomeBubble : MonoBehaviour
{
    private Button _bubble;
    public BiomeType BiomeType { get; private set; }

    private void Awake()
    {
        _bubble = GetComponentInChildren<Button>();
        _bubble.onClick.AddListener(StartBiomeRelation);
    }

    public void HideThisBubble(bool playDialogue = true) 
    {
        if (playDialogue) RoboDialogueManager.Instance.StartRoboDialogue("BiomeRelationComplete");
        RelationUIManager.Instance.HideBook();
        Destroy(gameObject);
    }

    public void SetBiomeType(BiomeType biomeType)
    {
        BiomeType = biomeType;
        if (!AnchorPoint.BiomeFinished.ContainsKey(BiomeType)) AnchorPoint.BiomeFinished[BiomeType] = new();
        AnchorPoint.BiomeFinished[BiomeType].AddListener(delegate { HideThisBubble(); }) ;
    }

    private void StartBiomeRelation()
    {
        ResourceGame.Instance.UpdateLevelBubbleBiome(BiomeType);
        ResourceGame.Instance.ProcessActiveScene(LevelSceneFlow.RelationPhase);
    }
}
