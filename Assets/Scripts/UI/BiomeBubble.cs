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

    public void SetBiomeType(BiomeType biomeType) 
    {
        BiomeType = biomeType;
    }

    private void StartBiomeRelation()
    {
        ResourceGame.Instance.UpdateLevelBubbleBiome(BiomeType);
        ResourceGame.Instance.ProcessActiveScene(LevelSceneFlow.RelationPhase);
    }
}
