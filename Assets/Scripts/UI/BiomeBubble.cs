using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BiomeBubble : MonoBehaviour
{
    private Button _bubble;
    public BiomeType BiomeType { get; private set; }
    public Sprite CompletedSprite;
    public Sprite DefaultSprite;
    public Image BubbleImage;
    public Image BiomeImage;
    private void Awake()
    {
        _bubble = GetComponentInChildren<Button>();
        _bubble.onClick.AddListener(StartBiomeRelation);
        BubbleImage.sprite = DefaultSprite;
    }

    public void BubbleComplete(bool playDialogue = true) 
    {
        if (playDialogue) RoboDialogueManager.Instance.StartRoboDialogue("BiomeRelationComplete");
        RelationUIManager.Instance.HideBook();
        BubbleImage.sprite = CompletedSprite;
    }

    public void SetBiomeType(BiomeType biomeType, Sprite sprite)
    {
        BiomeType = biomeType;
        BiomeImage.sprite = sprite;
        if (!AnchorPoint.BiomeFinished.ContainsKey(BiomeType)) AnchorPoint.BiomeFinished[BiomeType] = new();
        AnchorPoint.BiomeFinished[BiomeType].AddListener(delegate { BubbleComplete(); }) ;
    }

    private void StartBiomeRelation()
    {
        ResourceGame.Instance.UpdateLevelBubbleBiome(BiomeType);
        ResourceGame.Instance.ProcessActiveScene(LevelSceneFlow.RelationPhase);
    }
}
