using AnKuchen.Map;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    private List<DataUiElements> _dataUI;
    private ExIconsUiElements _exIconUI;
    private DiagnosisTitleUiElements _diagnosticsTitleUI;
    private DiagnosisInfoUiElements _diasgnosticsInfoUI;

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

        _diasgnosticsInfoUI = new DiagnosisInfoUiElements(diagnosticsInfoMapper);
        _diasgnosticsInfoUI.Root.SetActive(true);

        UpdateProvider(provider);
    }

    public void UpdateProvider(BookInfoProvider provider) 
    {
        _problemCount = provider.EnviroProblems.Count;
        List<EnviroProblemProvider> problems = provider.EnviroProblems;

        _photoUI.Photo.sprite = provider.BiomeSprite;
        for (int i = 0; i < _problemCount; i++)
        {
            _dataUI[i].Section.text = provider.GetEnviroProblemSectionString(problems[i].Section);
            _dataUI[i].Text.text = problems[i].Title;
            _dataUI[i].Icon.sprite = problems[i].Icon;

            _diasgnosticsInfoUI.Texts[i].text = problems[i].Description;
            _diasgnosticsInfoUI.Sprites[i].sprite = problems[i].Sprite;
        }

        for (int i = _problemCount; i < _dataUI.Count; i++)
        {
            _dataUI[i].Section.enabled = false;
            _dataUI[i].Text.enabled = false;
            _dataUI[i].Icon.enabled = false;

            _diasgnosticsInfoUI.Texts[i].enabled = false;
            _diasgnosticsInfoUI.Sprites[i].enabled = false;
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
    public List<Image> ExIcons { get; private set; }

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
    public List<TextMeshProUGUI> Texts { get; private set; }
    public List<Image> Sprites { get; private set; }

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

    }
}
