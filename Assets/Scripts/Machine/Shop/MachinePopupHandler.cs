using AnKuchen.Map;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public CanvasGroup CanvasGroup { get; private set; }
    public ShopPopupCanvasUiElements() { }
    public ShopPopupCanvasUiElements(IMapper mapper) { Initialize(mapper); }

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
        var usesCanvas = Uses.GetComponent<CanvasGroup>();
        var progressCanvas = Progress.GetComponent<CanvasGroup>();
        if (machine.RestrictionType == MachineRestrictionType.Limited_Placing)
        {
            usesCanvas.alpha = 1.0f;
            usesCanvas.interactable = true;
            usesCanvas.blocksRaycasts = true;
            Progress.text = BiomePhaseHandler.Instance.MachinePlaceRestrictionCount[machine.Type]  + "/" + machine.RestrictionTier;
        }
        else 
        {
            usesCanvas.alpha = 0.0f;
            usesCanvas.interactable = false;
            usesCanvas.blocksRaycasts = false;
        }
        if (machine.RangePattern == null) 
        {
            progressCanvas.alpha = 1.0f;
            progressCanvas.interactable = true;
            progressCanvas.blocksRaycasts = true;
            Progress.text = machine.CompletionRateModifier + "%";
        }
        else
        {
            progressCanvas.alpha = 0.0f;
            progressCanvas.interactable = false;
            progressCanvas.blocksRaycasts = false;
        }
    }

}
