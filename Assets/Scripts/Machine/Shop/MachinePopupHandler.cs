using AnKuchen.Map;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MachinePopupHandler : MonoBehaviour
{
    [SerializeField] private UICache _popupCache;

    public static MachinePopupHandler Instance;

    private ShopPopupCanvasUiElements _popupHandler;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        _popupHandler = new ShopPopupCanvasUiElements(_popupCache);
        _popupHandler.Root.SetActive(true);
        _popupHandler.HidePopup();
    }

    public void ShowPopUp(Machine machine, UnityEvent acceptEvent) { _popupHandler.ShowPopup(machine, acceptEvent); }
    public void HidePopUp() { _popupHandler.HidePopup(); }
}

public class ShopPopupCanvasUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Button CloseButton { get; private set; }
    public Button Buy { get; private set; }
    public TextMeshProUGUI Title { get; private set; }
    public TextMeshProUGUI Description { get; private set; }
    public TextMeshProUGUI Progress { get; private set; }
    public TextMeshProUGUI Uses { get; private set; }
    public TextMeshProUGUI Fail_Text { get; private set; }
    public CanvasGroup CanvasGroup { get; private set; }
    public ShopPopupCanvasUiElements() { }
    public ShopPopupCanvasUiElements(IMapper mapper) { Initialize(mapper); }

    public static Color32 redColor = new(255, 127, 63, 255);
    public static Color32 orangeColor = new(224,131,39, 255);
    public static Color32 greenColor = new (14,164,114, 255);

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        CloseButton = mapper.Get<Button>("CloseButton");
        Buy = mapper.Get<Button>("./Buy");
        Title = mapper.Get<TextMeshProUGUI>("Title");
        Description = mapper.Get<TextMeshProUGUI>("Desc");
        Progress = mapper.Get<TextMeshProUGUI>("Progress%");
        Uses = mapper.Get<TextMeshProUGUI>("UsesProgress");
        Fail_Text = mapper.Get<TextMeshProUGUI>("Fail_Text");

        CanvasGroup = Root.GetComponent<CanvasGroup>();

        CloseButton.onClick.AddListener(HidePopup);
        CloseButton.onClick.AddListener(MachineShop.Instance.EnableShopItems);
    }

    public void HidePopup()
    {
        CanvasGroup.alpha = 0.0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        IsUsingUI.IsUsingPopUp = false;
    }

    public void ShowPopup(Machine machine, UnityEvent acceptEvent)
    {
        IsUsingUI.IsUsingPopUp = true;
        CanvasGroup.alpha = 1.0f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        UpdateUI(machine, acceptEvent);
    }

    public void UpdateUI(Machine machine, UnityEvent acceptEvent) 
    {
        Buy.onClick.RemoveAllListeners();
        Buy.onClick.AddListener(acceptEvent.Invoke);
        Buy.onClick.AddListener(HidePopup);

        Title.text = machine.title;
        Description.text = machine.Description;

        if (machine.RestrictionType == MachineRestrictionType.Limited_Placing)
        {
            EnableUsesCanvas();
            DisableRiskCanvas();
            var useCount = BiomePhaseHandler.Instance.MachinePlaceRestrictionCount[machine.Type];
            var useProportion = machine.RestrictionTier == 0 ? 0 : useCount / machine.RestrictionTier;
            Uses.color = GetProportionColor(useProportion);
            Uses.text = useCount + "/" + machine.RestrictionTier;
            Uses.SetAllDirty();
            Uses.ForceMeshUpdate();
        }
        else if (machine.RestrictionType == MachineRestrictionType.Gambling)
        {
            EnableRiskCanvas();
            DisableUsesCanvas();
            Fail_Text.text = GetRiskText(machine);
            Fail_Text.color = GetRiskColor(machine);
            Fail_Text.SetAllDirty();
            Fail_Text.ForceMeshUpdate();
        }
        else 
        {
            DisableUsesCanvas();
            DisableRiskCanvas();
        }

        if (machine.RangePattern == null) 
        {
            EnableProgressCanvas();
            Progress.text = machine.CompletionRateModifier + "%";
        }
        else
        {
            DisableProgressCanvas();
        }
    }

    public void EnableRiskCanvas() 
    {
        var riskCanvas = Fail_Text.GetComponentInParent<CanvasGroup>();
        riskCanvas.alpha = 1.0f;
        riskCanvas.interactable = true;
        riskCanvas.blocksRaycasts = true;
    }
    public void DisableRiskCanvas() 
    {
        var riskCanvas = Fail_Text.GetComponentInParent<CanvasGroup>();
        riskCanvas.alpha = 0.0f;
        riskCanvas.interactable = false;
        riskCanvas.blocksRaycasts = false;
    }
    public void EnableProgressCanvas() 
    {
        var progressCanvas = Progress.GetComponentInParent<CanvasGroup>();

        progressCanvas.alpha = 1.0f;
        progressCanvas.interactable = true;
        progressCanvas.blocksRaycasts = true;

    }
    public void DisableProgressCanvas() 
    {
        var progressCanvas = Progress.GetComponentInParent<CanvasGroup>();
        progressCanvas.alpha = 0.0f;
        progressCanvas.interactable = false;
        progressCanvas.blocksRaycasts = false;
    }
    public void EnableUsesCanvas() 
    {
        var usesCanvas = Uses.GetComponentInParent<CanvasGroup>();
        usesCanvas.alpha = 1.0f;
        usesCanvas.interactable = true;
        usesCanvas.blocksRaycasts = true;
    }
    public void DisableUsesCanvas() 
    {
        var usesCanvas = Uses.GetComponentInParent<CanvasGroup>();
        usesCanvas.alpha = 0.0f;
        usesCanvas.interactable = false;
        usesCanvas.blocksRaycasts = false;
    }

    public string GetRiskText(Machine machine)
    {
        if (machine.RestrictionTier == 0) return "Bajo";
        if (machine.RestrictionTier == 1) return "Medio";
        if (machine.RestrictionTier == 2) return "Alto";
        else return "ERROR";
    }

    public Color32 GetRiskColor(Machine machine) 
    {
        if (machine.RestrictionTier == 0) return greenColor;
        if (machine.RestrictionTier == 1) return orangeColor;
        if (machine.RestrictionTier == 2) return redColor;
        else return Color.white;
    }

    public Color32 GetProportionColor(float proportion) 
    {
        if (proportion < 0.5f) return greenColor;
        if (proportion > 0.5f && proportion < 0.9f) return orangeColor;
        else return redColor;
    }
}
