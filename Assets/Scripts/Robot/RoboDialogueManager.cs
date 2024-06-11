using DUJAL.Systems.Dialogue;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private Image backgroundBlocker;
    [SerializeField] private CanvasGroup TextCanvasGroup;
    private void Start()
    {
        InspectorDialogue dialogueManager = InspectorDialogue.Instance;
        dialogueManager.Exit.AddListener(DisableDialogue);
        dialogueManager.Enter.AddListener(EnableDialogue);
    }

    public void EnableDialogue() 
    {
        IsUsingUI.IsUsingDialogue = true;
        backgroundBlocker.enabled = true;
        TextCanvasGroup.alpha = 1;
        TextCanvasGroup.blocksRaycasts = true;
        TextCanvasGroup.interactable= true;
    }

    public void DisableDialogue() 
    {
        IsUsingUI.IsUsingDialogue = false;
        backgroundBlocker.enabled = false;
        TextCanvasGroup.alpha = 0;
        TextCanvasGroup.blocksRaycasts = false;
        TextCanvasGroup.interactable = false;
    }

    public void StartRoboDialogue(string dialogueName) 
    {
        DialogueProvider dialogue;
        DialogueTrees.TryGetValue(dialogueName, out dialogue);
        if (dialogue.Dialogue == null) Debug.LogError($"Could not find dialogue: {dialogueName}");
        InspectorDialogue.Instance.Exit.AddListener(dialogue.DialogueEndEvent.Invoke);
        InspectorDialogue.Instance.Exit.AddListener(delegate { InspectorDialogue.Instance.Exit.RemoveListener(dialogue.DialogueEndEvent.Invoke); });
        InspectorDialogue.Instance.SetDialogue(dialogue.Dialogue, 0.1f);
        InspectorDialogue.Instance.HandleDialogueStart();
        InspectorDialogue.Instance.PlayText();
    }
}
