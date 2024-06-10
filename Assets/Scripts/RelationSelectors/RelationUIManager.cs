using AnKuchen.Map;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RelationUIManager : MonoBehaviour
{
    public static RelationUIManager Instance { get; private set; }
    public readonly Dictionary<LineSpawner, List<AnchorPoint>> NodeDictionary = new();

    [field: SerializeField] private UICache relationUICache = default;
    private GraphicRaycaster _raycaster;
    private RelationBookUiElements _relationBooks;
    private readonly List<TextMeshProUGUI> _consecuenceTexts = new();
    private readonly List<TextMeshProUGUI> _problemTexts = new();

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _relationBooks = new RelationBookUiElements(relationUICache, this);
        _relationBooks.Root.SetActive(true);

        _raycaster = GetComponent<GraphicRaycaster>();
        
        List<LineSpawner> spawners = GetComponentsInChildren<LineSpawner>().ToList();
        foreach (LineSpawner spawner in spawners) 
        {
            List<AnchorPoint> _nodes = spawner.GetComponentsInChildren<AnchorPoint>().ToList();
            NodeDictionary[spawner] = _nodes;
            
            var Text = spawner.GetComponentInChildren<TextMeshProUGUI>();
            if (_nodes[0].IsProblem(_nodes[0].NodeType)) _problemTexts.Add(Text);
            else _consecuenceTexts.Add(Text);
        }

    }

    public List<AnchorPoint> RaycastNodes(Vector3 mousePos) 
    {
        PointerEventData _PointerEventData = new(EventSystem.current)
        {
            position = mousePos
        };

        List<RaycastResult> results = new();
        _raycaster.Raycast(_PointerEventData, results);

        List<AnchorPoint> returnList = new();
        foreach (RaycastResult result in results)
        {
            AnchorPoint node = result.gameObject.GetComponentInParent<AnchorPoint>();
            if (node != null) returnList.Add(node); 
        }

        return returnList;
    }

    public void StartPaper(BookInfoProvider provider) 
    {
        Dictionary<EnviroProblemProvider, List<EnviroConsequence>> problemConsequencesDict = provider.GetFilteredConsequences(provider.EnviroProblems);
        List<EnviroConsequence> allConsequences = new();
        foreach (EnviroProblemProvider problem in problemConsequencesDict.Keys)
        {
            foreach (EnviroConsequence consequence in problemConsequencesDict[problem])
            {
                if (!allConsequences.Contains(consequence)) allConsequences.Add(consequence);
            }
        }
        if (allConsequences.Count > 4) Debug.LogError("ERROR: TOO MANY CONSEQUENCES PER BIOME");


        UpdatePaper(allConsequences, provider.EnviroProblems);
    }

    public void UpdatePaper(List<EnviroConsequence> consequences, List<EnviroProblemProvider> problems) 
    {
        for(int i = 0; i < consequences.Count; i++) 
        {
            var spawner = _consecuenceTexts[i].GetComponentInParent<LineSpawner>();
            var nodes = NodeDictionary[spawner];
            foreach (AnchorPoint node in nodes)
            {
                node.RelatedProblems = consequences[i].RelatedProblems;
                node.SetDataType((int) consequences[i].Type);
            }
            _consecuenceTexts[i].text = consequences[i].Title;
        }
        
        for(int i = 0; i < problems.Count; i++) 
        {
            var spawner = _problemTexts[i].GetComponentInParent<LineSpawner>();
            var nodes = NodeDictionary[spawner];
            foreach (AnchorPoint node in nodes)
            {
                node.RelatedConsequences = problems[i].Consequences;
                node.RelatedProblems = problems[i].Problems;
                node.SetDataType((int)problems[i].Type);
            }
            _problemTexts[i].text = problems[i].Title;
        }
        
        for(int i = consequences.Count; i < _consecuenceTexts.Count;i++) 
        {
            _consecuenceTexts[i].GetComponentInParent<LineSpawner>().enabled = false;
        }
        
        for(int i = problems.Count; i < _problemTexts.Count;i++) 
        {
            _problemTexts[i].GetComponentInParent<LineSpawner>().enabled = false;
        }
    }

    public void DisplayBook() 
    {
        _relationBooks.EnableBookMenu();
    }

}
public class RelationBookUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Button BookArrow { get; private set; }
    public Button BookArrow2 { get; private set; }
    public Button CloseButton { get; private set; }
    public Button PaperScreenArrow { get; private set; }
    public Button PaperScreenArrow2 { get; private set; }
    public CanvasGroup BookGroup { get; private set; }
    public CanvasGroup PaperGroup { get; private set; }
    
    public RelationBookUiElements() { }
    public RelationBookUiElements(IMapper mapper, RelationUIManager manager) { Initialize(mapper, manager); }

    private RelationUIManager Manager;

    public void Initialize(IMapper mapper, RelationUIManager manager)
    {
        Mapper = mapper;
        Manager = manager;
        Root = mapper.Get();
        BookArrow = mapper.Get<Button>("Book/Arrow");
        BookArrow2 = mapper.Get<Button>("Book/Arrow2");
        CloseButton = mapper.Get<Button>("CloseButton");
        PaperScreenArrow = mapper.Get<Button>("Paper Screen/Arrow");
        PaperScreenArrow2 = mapper.Get<Button>("Paper Screen/Arrow2");
        BookGroup = mapper.Get<CanvasGroup>("Book");
        PaperGroup = mapper.Get<CanvasGroup>("Paper Screen");

        BookArrow.onClick.AddListener(ShowPaper);
        BookArrow2.onClick.AddListener(ShowPaper);

        PaperScreenArrow.onClick.AddListener(ShowBook);
        PaperScreenArrow2.onClick.AddListener(ShowBook);
        
        CloseButton.onClick.AddListener(DisableBookMenu);

        HidePaper();
        HideBook();
        DisableCloseButton();
        DisableBookMenu();
    }

    public void EnableBookMenu(bool showCloseButton = true) 
    {
        ShowBook();
        if (showCloseButton) EnableCloseButton();

        Manager.GetComponent<CanvasGroup>().interactable = true;
        Manager.GetComponent<CanvasGroup>().alpha = 1.0f;
        Manager.GetComponent<CanvasGroup>().blocksRaycasts = true;

        CameraManager.Instance.SetBookCamera();
    }

    public void DisableBookMenu() 
    {
        HideBook();
        HidePaper();
        DisableCloseButton();
        Manager.GetComponent<CanvasGroup>().alpha = 0.0f;
        Manager.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Manager.GetComponent<CanvasGroup>().interactable = false;

        CameraManager.Instance?.SetGameplayCamera();
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
        ShowLineRenderers();
        PaperGroup.interactable = true;
        PaperGroup.alpha = 1.0f;
        PaperGroup.blocksRaycasts = true;
    }

    public void HidePaper() 
    {
        HideLineRenderers();
        PaperGroup.interactable = false;
        PaperGroup.alpha = 0.0f;
        PaperGroup.blocksRaycasts = false;
    }

    private void HideLineRenderers() 
    {
        foreach (List<AnchorPoint> nodes in Manager.NodeDictionary.Values) 
        {
            foreach (AnchorPoint node in nodes) 
            {
                var lines = node.GetComponentsInChildren<LineRenderer>();
                foreach (LineRenderer line in lines) line.enabled = false;
            }
        }
    }
    
    private void ShowLineRenderers() 
    {
        foreach (List<AnchorPoint> nodes in Manager.NodeDictionary.Values) 
        {
            foreach (AnchorPoint node in nodes) 
            {
                var lines = node.GetComponentsInChildren<LineRenderer>();
                foreach (LineRenderer line in lines) line.enabled = true;
            }
        }
    }

    private void DisableCloseButton()
    {
        var group = CloseButton.GetComponent<CanvasGroup>();
        group.interactable = false;
        group.alpha = 0.0f;
        group.blocksRaycasts = false;
    }
    private void EnableCloseButton()
    {
        var group = CloseButton.GetComponent<CanvasGroup>();
        group.interactable = true;
        group.alpha = 1.0f;
        group.blocksRaycasts = true;
    }

    public void Initialize(IMapper mapper)
    {
        throw new System.NotImplementedException();
    }
}


