using AnKuchen.Map;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void UpdateBook(BookInfoProvider provider) 
    {
        _bookUI.UpdateProvider(provider);
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

        _photoUI.Photo.sprite = _provider.BiomeSprite;
        
        int scrollbarVal = (int) (_diagnosticsInfoUI.Scrollbar.value * _diagnosticsInfoUI.Scrollbar.numberOfSteps);
        if (scrollbarVal >= 2.5) scrollbarVal -= 1;

        _diagnosticsTitleUI.Text.text = problems[scrollbarVal].Title;
        for (int i = 0; i < problems[scrollbarVal].Descriptions.Count; i++)
        {
            _diagnosticsInfoUI.Texts[i].text = problems[scrollbarVal].Descriptions[i];
            _diagnosticsInfoUI.Sprites[i].sprite = problems[scrollbarVal].Sprites[i];
        }

        for (int i = _problemCount; i < _diagnosticsInfoUI.Texts.Count; i++)
        {
            _diagnosticsInfoUI.Texts[i].text = problems[scrollbarVal].Descriptions[i];
            _diagnosticsInfoUI.Sprites[i].sprite = problems[scrollbarVal].Sprites[i];
        }
    }

    public void UpdateProvider(BookInfoProvider provider) 
    {
        _provider = provider;
        _problemCount = _provider.EnviroProblems.Count;
        List<EnviroProblemProvider> problems = _provider.EnviroProblems;

        UpdateScrollBar();

        for (int i = 0; i < _problemCount; i++)
        {
            _dataUI[i].Section.text = _provider.GetEnviroProblemSectionString(problems[i].Section);
            _dataUI[i].Text.text = problems[i].Title;
            _dataUI[i].Icon.sprite = problems[i].Icon;
        }

        for (int i = _problemCount; i < _dataUI.Count; i++)
        {
            _dataUI[i].Section.enabled = false;
            _dataUI[i].Text.enabled = false;
            _dataUI[i].Icon.enabled = false;
        }
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
    public Image Photo { get; private set; }
    public PhotoUiElements() { }
    public PhotoUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Photo = mapper.Get<Image>("Base");
    }
}

public class TitleUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public TextMeshProUGUI Title { get; private set; }
    public TitleUiElements() { }
    public TitleUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Title = mapper.Get<TextMeshProUGUI>("TitleText");
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
    public Image Icon { get; private set; }
    public TextMeshProUGUI Text { get; private set; }
    public TextMeshProUGUI Section { get; private set; }
    public DataUiElements() { }
    public DataUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Icon = mapper.Get<Image>("Icon");
        Text = mapper.Get<TextMeshProUGUI>("Text");
        Section = mapper.Get<TextMeshProUGUI>("Tipe");
    }
}

public class ExIconsUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public List<Image> ExIcons { get; private set; } = new();

    public ExIconsUiElements() { }
    public ExIconsUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        ExIcons.Add(mapper.Get<Image>("Ex_Icon1"));
        ExIcons.Add(mapper.Get<Image>("Ex_Icon2"));
        ExIcons.Add(mapper.Get<Image>("Ex_Icon3"));
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
    public List<Image> Sprites { get; private set; } = new();
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

        Sprites.Add(mapper.Get<Image>("Diag_Image1"));
        Sprites.Add(mapper.Get<Image>("Diag_Image2"));
        Sprites.Add(mapper.Get<Image>("Diag_Image3"));

        Scrollbar = mapper.Get<Scrollbar>("Scrollbar");

    }
}
