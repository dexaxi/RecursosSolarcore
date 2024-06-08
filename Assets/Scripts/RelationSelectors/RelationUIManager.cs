using AnKuchen.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationUIManager : MonoBehaviour
{
    [field: SerializeField] private UICache relationUICache = default;

    private void Start()
    {
        var relationBook = new RelationBookUiElements(relationUICache);
        relationBook.Root.SetActive(true);

    }

}
public class RelationBookUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Button NextLevel { get; private set; }
    public Button BookArrow { get; private set; }
    public Button BookArrow2 { get; private set; }
    public Button CloseButton { get; private set; }
    public Button PaperScreenArrow { get; private set; }
    public Button PaperScreenArrow2 { get; private set; }
    public Button BTN_1_Button { get; private set; }
    public CanvasGroup BookGroup { get; private set; }
    public CanvasGroup PaperGroup { get; private set; }
    public CanvasGroup DialogueGroup { get; private set; }
    
    public RelationBookUiElements() { }
    public RelationBookUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        NextLevel = mapper.Get<Button>("NextLevel");
        BookArrow = mapper.Get<Button>("Book/Arrow");
        BookArrow2 = mapper.Get<Button>("Book/Arrow2");
        CloseButton = mapper.Get<Button>("CloseButton");
        PaperScreenArrow = mapper.Get<Button>("Paper Screen/Arrow");
        PaperScreenArrow2 = mapper.Get<Button>("Paper Screen/Arrow2");
        BTN_1_Button = mapper.Get<Button>("BTN_1_Button");
        BookGroup = mapper.Get<CanvasGroup>("Book");
        PaperGroup = mapper.Get<CanvasGroup>("Paper Screen");
        DialogueGroup = mapper.Get<CanvasGroup>("RoboDialogue");

        BookArrow.onClick.AddListener(ShowPaper);
        BookArrow2.onClick.AddListener(ShowPaper);

        PaperScreenArrow.onClick.AddListener(ShowBook);
        PaperScreenArrow2.onClick.AddListener(ShowBook);
        HidePaper();
    }

    public void ShowBook() 
    {
        HidePaper();
        BookGroup.interactable = true;
        BookGroup.alpha = 1.0f;
        BookGroup.blocksRaycasts = true;
    }

    public void HideBook() 
    {
        BookGroup.interactable = false;
        BookGroup.alpha = 0.0f;
        BookGroup.blocksRaycasts = false;
    }
    public void ShowPaper() 
    {
        HideBook();
        PaperGroup.interactable = true;
        PaperGroup.alpha = 1.0f;
        PaperGroup.blocksRaycasts = true;
    }

    public void HidePaper() 
    {
        PaperGroup.interactable = false;
        PaperGroup.alpha = 0.0f;
        PaperGroup.blocksRaycasts = false;
    }
}


