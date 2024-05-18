using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.Events;
public class GenericPopUp : MonoBehaviour
{
    [Header("Text Data")]
    public string PopUpHeaderText;
    public string PopUpBodyText;

    [Header("Color Data")]
    [SerializeField] public Color PrimaryBackground;
    [SerializeField] public Color HeaderBackground;
    [SerializeField] public Color BodyBackground;
    [SerializeField] public Color HeaderTextColor;
    [SerializeField] public Color BodyTextColor;

    [Header("General References")]
    private CanvasGroup _popupCanvasGroup;
    [SerializeField] TextMeshProUGUI HeadText;
    [SerializeField] Button CloseButton;
    [Space(20)]
    [SerializeField] UnityEvent CloseCallback;
    [SerializeField] public Image Background;
    [SerializeField] public Image HeaderBackgroundImage;

    [Header("Normal Pop-Up References")]
    [SerializeField] CanvasGroup InfoCanvasGroup;
    [SerializeField] TextMeshProUGUI InfoBodyText;
    [SerializeField] public Image InfoBackground;

    // Option Pop-Up
    [Header("Option Pop-Up References")]
    [SerializeField] CanvasGroup OptionCanvasGroup;
    [SerializeField] TextMeshProUGUI OptionBodyText;
    [SerializeField] public Image OptionBackground;
    [SerializeField] Button CancelButton;
    [SerializeField] Button AcceptButton;
    [Space(20)]
    [SerializeField] UnityEvent AcceptCallback;
    [SerializeField] UnityEvent CancelCallback;

    [SerializeField] Sprite backGroundSprite;
    [SerializeField] Sprite headerSprite;
    [SerializeField] Sprite bodySprite;

    private void Awake()
    {
        _popupCanvasGroup = GetComponent<CanvasGroup>();
        CloseButton.onClick.AddListener(ClosePopUp);
        AcceptButton.onClick.AddListener(ClosePopUp);
        CancelButton.onClick.AddListener(ClosePopUp);

        // DEBUG DATA
        /*
        UnityEvent closeEvent = new();
        closeEvent.AddListener(DebugClose);
        UnityEvent acceptEvent = new();
        acceptEvent.AddListener(DebugAccept);
        UnityEvent cancelEvent = new();
        cancelEvent.AddListener(DebugCancel);
        //BuildInfoPopupPlainColor("Error!", "Han pasado cosas, friki", 1, new Color(155,155,155, 0.3f), new Color(0, 155, 155 , 0.6f), new Color(0, 155, 155, 0.6f), Color.white, Color.white, closeEvent);
        //BuildInfoPopupSprite("Error!", "Han pasado cosas, friki", 1, backGroundSprite, headerSprite, bodySprite, Color.white, Color.white, closeEvent);
        //BuildOptionPopupPlainColor("Error!", "Han pasado cosas, friki", 2, new Color(155, 155, 155, 0.3f), new Color(0, 155, 155, 0.6f), new Color(0, 155, 155, 0.6f), Color.white, Color.white, acceptEvent, cancelEvent);
        BuildOptionPopupSprite("Error!", "Han pasado cosas, friki", 1, backGroundSprite, headerSprite, bodySprite, Color.white, Color.white, acceptEvent, cancelEvent);
        */
    }

    public void BuildInfoPopupPlainColor(string header, string body, float size, Color background, Color headerBackground, Color bodyBackground, Color headerText, Color bodyText, UnityEvent closeButtonCallback = null) 
    {
        BaseInfoTextBuilder(header, body, headerText, bodyText);

        // Backgrounds
        if (background == null) Background.color = PrimaryBackground;
        else Background.color = background;

        if (headerBackground == null) HeaderBackgroundImage.color = HeaderBackground;
        else HeaderBackgroundImage.color = headerBackground;

        if (bodyBackground == null) InfoBackground.color = BodyBackground;
        else InfoBackground.color = bodyBackground;

        HandlePopupSize(size);
        HandlePopupCallbacks(closeButtonCallback);
       
        EnableInfoPopUp();
    }

    public void BuildInfoPopupSprite(string header, string body, float size, Sprite background, Sprite headerBackground, Sprite bodyBackground, Color headerText, Color bodyText, UnityEvent closeButtonCallback = null)
    {
        BaseInfoTextBuilder(header, body, headerText, bodyText);

        // Backgrounds

        HeaderBackgroundImage.sprite = headerBackground;
        Background.sprite = background;
        InfoBackground.sprite = bodyBackground;

        HandlePopupSize(size);
        HandlePopupCallbacks(closeButtonCallback);

        EnableInfoPopUp();
    }
    public void BuildOptionPopupPlainColor(string header, string body, float size, Color background, Color headerBackground, Color bodyBackground, Color headerText, Color bodyText, UnityEvent cancelCallback = null, UnityEvent acceptCallback = null)
    {
        BaseOptionTextBuilder(header, body, headerText, bodyText);

        // Backgrounds
        if (background == null) Background.color = PrimaryBackground;
        else Background.color = background;

        if (headerBackground == null) HeaderBackgroundImage.color = HeaderBackground;
        else HeaderBackgroundImage.color = headerBackground;

        if (bodyBackground == null) OptionBackground.color = BodyBackground;
        else OptionBackground.color = bodyBackground;

        HandlePopupSize(size);
        HandlePopupCallbacks(null, acceptCallback, cancelCallback);

        EnableOptionPopUp();
    }

    public void BuildOptionPopupSprite(string header, string body, float size, Sprite background, Sprite headerBackground, Sprite bodyBackground, Color headerText, Color bodyText, UnityEvent cancelCallback = null, UnityEvent acceptCallback = null)
    {
        BaseOptionTextBuilder(header, body, headerText, bodyText);

        // Backgrounds
        HeaderBackgroundImage.sprite = headerBackground;
        Background.sprite = background;
        OptionBackground.sprite = bodyBackground;

        HandlePopupSize(size);
        HandlePopupCallbacks(null, acceptCallback, cancelCallback);

        EnableOptionPopUp();
    }

    private void BaseOptionTextBuilder(string header, string body, Color headerText, Color bodyText) 
    {
        if (header == null) HeadText.text = PopUpHeaderText;
        else HeadText.text = header;

        if (body == null) OptionBodyText.text = PopUpBodyText;
        else OptionBodyText.text = body;

        if (headerText == null) HeadText.color = HeaderTextColor;
        else HeadText.color = headerText;

        if (bodyText == null) OptionBodyText.color = BodyTextColor;
        else OptionBodyText.color = bodyText;
    }

    private void BaseInfoTextBuilder(string header, string body, Color headerText, Color bodyText) 
    {
        if (header == null) HeadText.text = PopUpHeaderText;
        else HeadText.text = header;

        if (body == null) InfoBodyText.text = PopUpBodyText;
        else InfoBodyText.text = body;

        if (headerText == null) HeadText.color = HeaderTextColor;
        else HeadText.color = headerText;

        if (bodyText == null) InfoBodyText.color = BodyTextColor;
        else InfoBodyText.color = bodyText;
    }

    private void HandlePopupCallbacks(UnityEvent closeButtonCallback = null, UnityEvent acceptCallback = null, UnityEvent cancelCallback = null) 
    {
        // UnityEvent inspector callbacks
        AcceptButton.onClick.AddListener(AcceptCallback.Invoke);
        CloseButton.onClick.AddListener(CloseCallback.Invoke);
        CancelButton.onClick.AddListener(CancelCallback.Invoke);

        // UnityEvent @param callbacks
        if (closeButtonCallback != null) CloseButton.onClick.AddListener(closeButtonCallback.Invoke);
        if (acceptCallback != null) AcceptButton.onClick.AddListener(acceptCallback.Invoke);
        if (cancelCallback != null) CancelButton.onClick.AddListener(cancelCallback.Invoke);
    }

    private void HandlePopupSize(float size) 
    {
        // Size
        this.transform.Scale(size);
    }

    private void EnableBasePopUp() 
    {
        IsUsingUI.IsUsingPopUp = true;
        _popupCanvasGroup.alpha = 1;
        _popupCanvasGroup.interactable = true;
        _popupCanvasGroup.blocksRaycasts = true;
    }
    private void EnableInfoPopUp() 
    {
        EnableBasePopUp();

        OptionCanvasGroup.alpha = 0;
        OptionCanvasGroup.interactable = false;
        OptionCanvasGroup.blocksRaycasts = false;
        
        InfoCanvasGroup.alpha = 1;
        InfoCanvasGroup.interactable = true;
        InfoCanvasGroup.blocksRaycasts = true;
    }

    private void EnableOptionPopUp() 
    {
        EnableBasePopUp();
        CloseButton.gameObject.SetActive(false);
        InfoCanvasGroup.alpha = 0;
        InfoCanvasGroup.interactable = false;
        InfoCanvasGroup.blocksRaycasts = false;

        OptionCanvasGroup.alpha = 1;
        OptionCanvasGroup.interactable = true;
        OptionCanvasGroup.blocksRaycasts = true;

    }

    private void ClosePopUp()
    {
        IsUsingUI.IsUsingPopUp = false;
        _popupCanvasGroup.alpha = 0;
        _popupCanvasGroup.interactable = false;
        _popupCanvasGroup.blocksRaycasts = false;
        Invoke("DestroyPopUp", 1);
    }

    private void DestroyPopUp()
    {
        Destroy(gameObject);
    }

    private void DebugClose() { Debug.Log("Closed!"); }
    private void DebugAccept() { Debug.Log("Accepted!"); }
    private void DebugCancel() { Debug.Log("Canceled!"); }
}
