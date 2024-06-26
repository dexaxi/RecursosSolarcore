using AnKuchen.Map;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class BookUIManager : MonoBehaviour
{
    public static BookUIManager Instance;

    [field: SerializeField] private UICache bookUICache = default;
    [field: SerializeField] private UICache photoUICache = default;
    [field: SerializeField] private UICache titleUICache = default;
    [field: SerializeField] private UICache dataInfoUICache = default;
    [field: SerializeField] private List<UICache> dataUICache = default;
    [field: SerializeField] private UICache exIconUICache = default;
    [field: SerializeField] private UICache diagnosticsTitleUICache = default;
    [field: SerializeField] private UICache diagnosticsInfoUICache= default;

    private BookUiElements _bookUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartBook(BookInfoProvider provider)
    {
        _bookUI = new BookUiElements(bookUICache, photoUICache, titleUICache, dataInfoUICache,
            dataUICache, exIconUICache, diagnosticsTitleUICache, diagnosticsInfoUICache, provider);
        _bookUI.Root.SetActive(true);
    }
}
public class BookUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Button Arrow { get; private set; }
    public Button Arrow2 { get; private set; }

    public CanvasGroup CanvasGroup { get; private set; }

    private int _problemCount;
    BookInfoProvider _provider;

    public BookUiElements() { }
    public BookUiElements(IMapper mapper, 
        UICache photoMapper,
        UICache titleMapper,
        UICache dataInfoMapper,
        List<UICache> dataMapper,
        UICache exMapper,
        UICache diagnosticsTitleMapper,
        UICache diagnosticsInfoMapper,
        BookInfoProvider provider) 
    { 
        Initialize(mapper, photoMapper, titleMapper, dataInfoMapper, dataMapper, 
            exMapper, diagnosticsTitleMapper, diagnosticsInfoMapper, provider); 
    }

    private PhotoUiElements _photoUI;
    private TitleUiElements _titleUI;
    private DataInfoUiElements _dataInfoUI;
    private List<DataUiElements> _dataUI = new();
    private ExIconsUiElements _exIconUI;
    private DiagnosisTitleUiElements _diagnosticsTitleUI;
    private DiagnosisInfoUiElements _diagnosticsInfoUI;
    public void Initialize(IMapper mapper,
        UICache photoMapper,
        UICache titleMapper,
        UICache dataInfoMapper,
        List<UICache> dataMapper,
        UICache exMapper,
        UICache diagnosticsTitleMapper,
        UICache diagnosticsInfoMapper,
        BookInfoProvider provider)
    {
        Mapper = mapper;
        Root = mapper.Get();
        CanvasGroup = Root.GetComponent<CanvasGroup>();

        _photoUI = new PhotoUiElements(photoMapper);
        _photoUI.Root.SetActive(true);

        _titleUI = new TitleUiElements(titleMapper);
        _titleUI.Root.SetActive(true);

        foreach (UICache cache in dataMapper)
        {
            DataUiElements _dataUIElement = new DataUiElements(cache);
            _dataUIElement.Root.SetActive(true);
            _dataUI.Add(_dataUIElement);
        }

        _dataInfoUI = new DataInfoUiElements(dataInfoMapper, _dataUI);
        _dataInfoUI.Root.SetActive(true);

        _exIconUI = new ExIconsUiElements(exMapper);
        _exIconUI.Root.SetActive(true);

        _diagnosticsTitleUI = new DiagnosisTitleUiElements(diagnosticsTitleMapper);
        _diagnosticsTitleUI.Root.SetActive(true);

        _diagnosticsInfoUI = new DiagnosisInfoUiElements(diagnosticsInfoMapper);
        _diagnosticsInfoUI.Root.SetActive(true);

        _diagnosticsTitleUI.Title.text = "Diagnostics: ";

        HandleScrollBarUpdate().Forget();
        UpdateProvider(provider);
    }

    private async UniTask HandleScrollBarUpdate() 
    {
        await UniTask.WaitUntilValueChanged(_diagnosticsInfoUI.Scrollbar, x => x.value);
        UpdateScrollBar();
        HandleScrollBarUpdate().Forget();
    }

    private void UpdateScrollBar() 
    {
        List<EnviroProblemProvider> problems = _provider.EnviroProblems;
        var tmproCount = _diagnosticsInfoUI.Texts.Count;
        var descriptionCount = problems[0].AlterationDescriptionList.Count;


        if ((float) descriptionCount / tmproCount <= 1.0f)
        {
            _diagnosticsInfoUI.Scrollbar.GetComponent<CanvasGroup>().alpha = 0.0f;
            _diagnosticsInfoUI.Scrollbar.GetComponent<CanvasGroup>().interactable = false;
            _diagnosticsInfoUI.Scrollbar.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            _diagnosticsInfoUI.Scrollbar.GetComponent<CanvasGroup>().alpha = 1.0f;
            _diagnosticsInfoUI.Scrollbar.GetComponent<CanvasGroup>().interactable = true;
            _diagnosticsInfoUI.Scrollbar.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        var nsteps = (int)((float)descriptionCount / tmproCount);
        _diagnosticsInfoUI.Scrollbar.numberOfSteps = nsteps == 1 ? nsteps + 1 : nsteps;
        _diagnosticsInfoUI.Scrollbar.size = 1.0f / _diagnosticsInfoUI.Scrollbar.numberOfSteps;

        int scrollbarVal = Mathf.RoundToInt(_diagnosticsInfoUI.Scrollbar.value + 0.01f * tmproCount);

        _diagnosticsTitleUI.Text.text = problems[0].AlterationTitle;
        for (int i = 0; i < tmproCount; i++)
        {
            if (i + (scrollbarVal * tmproCount) >= descriptionCount) continue; 
            _diagnosticsInfoUI.Texts[i].text = problems[0].AlterationDescriptionList[i + (scrollbarVal * tmproCount)];
            //_diagnosticsInfoUI.Sprites[i].sprite = problems[0].AlterationSpritesDescriptions[i + (scrollbarVal * tmproCount)];
            //_diagnosticsInfoUI.Sprites[i].enabled = true;
        }

        for (int i = 0; i < _diagnosticsInfoUI.Texts.Count; i++)
        {
            if (i + (scrollbarVal * tmproCount) >= descriptionCount) 
            {
                _diagnosticsInfoUI.Texts[i].text = "";
                //_diagnosticsInfoUI.Sprites[i].enabled = false;
            }
        }
    }

    public void UpdateProvider(BookInfoProvider provider) 
    {
        _provider = provider;
        _problemCount = _provider.EnviroProblems.Count;
        List<EnviroProblemProvider> problems = _provider.EnviroProblems;

        _photoUI.Photo.sprite = Sprite.Create(_provider.BiomeSprite, new Rect(0.0f, 0.0f, _provider.BiomeSprite.width, _provider.BiomeSprite.height), new Vector2(0.5f, 0.5f), 100.0f);
        _titleUI.Title.text = _provider.BiomeName;
        _titleUI.Info.onClick.RemoveAllListeners();
        _titleUI.Info.onClick.AddListener(delegate { RoboDialogueManager.Instance.StartRoboDialogue(provider.BiomeName + "Info"); });

        UpdateScrollBar();

        for (int i = 0; i < _problemCount; i++)
        {
            var phase = problems[i].Phase;
            _dataUI[phase].Section.enabled = true;
            _dataUI[phase].Text.enabled = true;
            _dataUI[phase].Icon.enabled = true;
            _dataUI[phase].Section.text = _provider.GetEnviroProblemSectionString(problems[i].Section);
            _dataUI[phase].Text.text = problems[i].Title;
            _dataUI[phase].Icon.sprite = BookInfoProvider.GetEnviroProblemIcon(problems[i].Section, _provider.BiomeType);
        }

        for (int i = _problemCount; i < _dataUI.Count; i++)
        {
            _dataUI[i].Section.enabled = false;
            _dataUI[i].Text.enabled = false;
            _dataUI[i].Icon.enabled = false;
        }

        var problemTypes = new List<EnviroProblemType>();
        foreach (var problem in problems)
        {
            problemTypes.Add(problem.Type);
        }
        _exIconUI.UpdateExIcons(problemTypes);
    }

    public void Initialize(IMapper mapper)
    {
        throw new System.NotImplementedException();
    }
}

public class PhotoUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public UnityEngine.UI.Image Photo { get; private set; }
    public PhotoUiElements() { }
    public PhotoUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Photo = mapper.Get<UnityEngine.UI.Image>("Background");
    }
}

public class TitleUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public TextMeshProUGUI Title { get; private set; }
    public Button Info { get; private set; }
    public TitleUiElements() { }
    public TitleUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Title = mapper.Get<TextMeshProUGUI>("TitleText");
        Info = mapper.Get<Button>("Info_Button");
    }
}

public class DataInfoUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public DataInfoUiElements() { }
    public DataInfoUiElements(IMapper mapper, List<DataUiElements> dataUI) { Initialize(mapper, dataUI); }
    public void Initialize(IMapper mapper, List<DataUiElements> dataUI)
    {
        Mapper = mapper;
        Root = mapper.Get();
    }
    public void Initialize(IMapper mapper)
    {
        throw new System.NotImplementedException();
    }
}

public class DataUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public UnityEngine.UI.Image Icon { get; private set; }
    public TextMeshProUGUI Text { get; private set; }
    public TextMeshProUGUI Section { get; private set; }
    public DataUiElements() { }
    public DataUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Icon = mapper.Get<UnityEngine.UI.Image>("Icon");
        Text = mapper.Get<TextMeshProUGUI>("Text");
        Section = mapper.Get<TextMeshProUGUI>("Tipe");
    }
}

public class ExIconsUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public List<UnityEngine.UI.Image> ExIcons { get; private set; } = new();
    public List<UnityEngine.UI.Image> CheckIcons { get; private set; } = new();

    public ExIconsUiElements() { }
    public ExIconsUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        ExIcons.Add(mapper.Get<UnityEngine.UI.Image>("Ex_Icon1"));
        ExIcons.Add(mapper.Get<UnityEngine.UI.Image>("Ex_Icon2"));
        ExIcons.Add(mapper.Get<UnityEngine.UI.Image>("Ex_Icon3"));
        CheckIcons.Add(mapper.Get<UnityEngine.UI.Image>("Val_Icon1"));
        CheckIcons.Add(mapper.Get<UnityEngine.UI.Image>("Val_Icon2"));
        CheckIcons.Add(mapper.Get<UnityEngine.UI.Image>("Val_Icon3"));

        foreach (var exicon in ExIcons) exicon.enabled = true;
        foreach (var checkicon in CheckIcons) checkicon.enabled = false;
    }

    public void UpdateExIcons(List<EnviroProblemType> problems) 
    {
        int i = 0;
        foreach (var problem in problems) 
        {
            int curCompletion = 0;
            ExIcons[i].enabled = true;
            bool hasVal = BiomePhaseHandler.Instance.CurrentCompletion.TryGetValue(problem, out curCompletion);
            if (hasVal && curCompletion >= 100)
            {
                ExIcons[i].enabled = false;
                CheckIcons[i].enabled = true;
            }
            else 
            {
                ExIcons[i].enabled = true;
                CheckIcons[i].enabled = false;
            }
            i++;
        }

        for (int j = problems.Count; j < ExIcons.Count; j++) 
        {
            ExIcons[j].enabled = false;
            CheckIcons[j].enabled = false;
        }
    }
}


public class DiagnosisTitleUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public TextMeshProUGUI Title { get; private set; }
    public TextMeshProUGUI Text { get; private set; }
    public DiagnosisTitleUiElements() { }
    public DiagnosisTitleUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Title = mapper.Get<TextMeshProUGUI>("Diagnosis");
        Text = mapper.Get<TextMeshProUGUI>("Diagnosis Title");
    }
}

public class DiagnosisInfoUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public List<TextMeshProUGUI> Texts { get; private set; } = new();
    public List<UnityEngine.UI.Image> Sprites { get; private set; } = new();
    public Scrollbar Scrollbar { get; private set; } 
    public DiagnosisInfoUiElements() { }
    public DiagnosisInfoUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();

        Texts.Add(mapper.Get<TextMeshProUGUI>("Diag_Text1"));
        Texts.Add(mapper.Get<TextMeshProUGUI>("Diag_Text2"));
        Texts.Add(mapper.Get<TextMeshProUGUI>("Diag_Text3"));

        Sprites.Add(mapper.Get<UnityEngine.UI.Image>("Diag_Image1"));
        Sprites.Add(mapper.Get<UnityEngine.UI.Image>("Diag_Image2"));
        Sprites.Add(mapper.Get<UnityEngine.UI.Image>("Diag_Image3"));

        Scrollbar = mapper.Get<Scrollbar>("Scrollbar");

    }
}
