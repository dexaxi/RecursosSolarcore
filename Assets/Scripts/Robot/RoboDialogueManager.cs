using DUJAL.Systems.Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public struct DialogueProvider 
{
    public DialogueContainerScriptableObject Dialogue;
    [Space(20)]
    public UnityEvent DialogueEndEvent;
    public DialogueProvider(DialogueContainerScriptableObject dialogue, UnityEvent dialogueEndEvent)
    {
        Dialogue = dialogue;
        DialogueEndEvent = dialogueEndEvent;
    }
}

public class RoboDialogueManager : MonoBehaviour
{
    public static RoboDialogueManager Instance;

    [SerializeField] public SerializableDictionary<string, DialogueProvider> DialogueTrees = new();

    [SerializeField] private Image backgroundBlocker;

    [SerializeField] TextMeshProUGUI gameplayDialogue;
    [SerializeField] TextMeshProUGUI relationDialogue;

    [SerializeField] private CanvasGroup RelationCanvasGroup;
    [SerializeField] CanvasGroup GameplayCanvasGroup;

    private SerializableDictionary<string, int> DialogueCounter = new();

    private int _maxLines;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        SwitchToRelationDialogue();
    }


    private TextMeshProUGUI _currentDialogueMesh;
    private CanvasGroup _currentCanvasGroup;


    private void Start()
    {
        InspectorDialogue dialogueManager = InspectorDialogue.Instance;
        dialogueManager.Exit.AddListener(DisableDialogue);
        dialogueManager.Enter.AddListener(EnableDialogue);
        GameplayCanvasGroup.alpha = 0;
        GameplayCanvasGroup.blocksRaycasts = false;
        GameplayCanvasGroup.interactable = false;
    }

    public void EnableDialogue() 
    {
        IsUsingUI.IsUsingDialogue = true;
        backgroundBlocker.enabled = true;
        _currentCanvasGroup.alpha = 1;
        _currentCanvasGroup.blocksRaycasts = true;
        _currentCanvasGroup.interactable= true;
    }

    public void DisableDialogue() 
    {
        IsUsingUI.IsUsingDialogue = false;
        backgroundBlocker.enabled = false;
        _currentCanvasGroup.alpha = 0;
        _currentCanvasGroup.blocksRaycasts = false;
        _currentCanvasGroup.interactable = false;
    }

    public void PlayOnce(string dialogueName) 
    {
        if (DialogueCounter.ContainsKey(dialogueName)) 
        {
            return;
        }
        DialogueCounter[dialogueName] = 1;
        StartRoboDialogue(dialogueName);
    }

    public void PlayNTimes(string dialogueName, int times) 
    {
        if (DialogueCounter.ContainsKey(dialogueName) && DialogueCounter[dialogueName] >= times) return;
        if (!DialogueCounter.ContainsKey(dialogueName)) DialogueCounter[dialogueName] = 0;
        else DialogueCounter[dialogueName]++;
        StartRoboDialogue(dialogueName);
    }

    public void StartRoboDialogue(string dialogueName) 
    {
        DialogueProvider dialogue;
        DialogueTrees.TryGetValue(dialogueName, out dialogue);
        if (dialogue.Dialogue == null) Debug.LogError($"Could not find dialogue: {dialogueName}");
        InspectorDialogue.Instance.Exit.AddListener(dialogue.DialogueEndEvent.Invoke);
        InspectorDialogue.Instance.Exit.AddListener(delegate { InspectorDialogue.Instance.Exit.RemoveListener(dialogue.DialogueEndEvent.Invoke); });
        InspectorDialogue.Instance.SetDialogue(dialogue.Dialogue, 0.05f, _currentDialogueMesh, _maxLines);
        InspectorDialogue.Instance.HandleDialogueStart();
        InspectorDialogue.Instance.PlayText();
    }

    public void SwitchToGameplayDialogue() 
    {
        _currentDialogueMesh = gameplayDialogue;
        _currentCanvasGroup = GameplayCanvasGroup;
        _maxLines = 9;
    }
    public void SwitchToRelationDialogue() 
    {
        _currentDialogueMesh = relationDialogue;
        _currentCanvasGroup = RelationCanvasGroup;
        _maxLines = 3;
    }
}
