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
    [SerializeField] UnityEvent CloseCallback;
    [SerializeField] public Image Background;
    [SerializeField] public Image HeaderBackgroundImage;

    [Header("Normal Pop-Up References")]
    [SerializeField] CanvasGroup GenericCanvasGroup;
    [SerializeField] TextMeshProUGUI GenericBodyText;
    [SerializeField] public Image GenericBackground;

    // Option Pop-Up
    [Header("Option Pop-Up References")]
    [SerializeField] CanvasGroup OptionCanvasGroup;
    [SerializeField] TextMeshProUGUI OptionBodyText;
    [SerializeField] public Image OptionBackground;
    [SerializeField] Button CancelButton;
    [SerializeField] Button AcceptButton;
    [SerializeField] UnityEvent AcceptCallback;
    [SerializeField] UnityEvent CancelCallback;

    private void Awake()
    {
        _popupCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void BuildInfoPopupPlainColor(string header, string body, float size, Color background, Color headerBackground, Color bodyBackground, Color headerText, Color bodyText, UnityEvent closeButtonCallback) 
    {
        // Text
        if (header == null) HeadText.text = PopUpHeaderText; 
        else HeadText.text = header;

        if (body == null) GenericBodyText.text = PopUpBodyText;
        else GenericBodyText.text = body;

        if (headerText == null) HeadText.color = HeaderTextColor;
        else HeadText.color = headerText;
        
        if (bodyText == null) GenericBodyText.color = BodyTextColor;
        else GenericBodyText.color = bodyText;

        // Backgrounds
        if (background == null) Background.color = PrimaryBackground;
        else Background.color = background;

        if (headerBackground == null) HeaderBackgroundImage.color = HeaderBackground;
        else HeaderBackgroundImage.color = headerBackground;

        if (bodyBackground == null) GenericBackground.color = BodyBackground;
        else GenericBackground.color = bodyBackground;

        // Extra
        this.transform.Scale(size);
        if (closeButtonCallback == null) CloseButton.onClick.AddListener(CloseCallback.Invoke);
        else CloseButton.onClick.AddListener(closeButtonCallback.Invoke);
        EnableInfoPopUp();
    }

    public void BuildInfoPopupSprite(string header, string body, float size, Sprite background, Sprite headerBackground, Sprite bodyBackground, Color headerText, Color bodyText, UnityEvent closeButtonCallback)
    {
        // Text
        if (header == null) HeadText.text = PopUpHeaderText;
        else HeadText.text = header;

        if (body == null) GenericBodyText.text = PopUpBodyText;
        else GenericBodyText.text = body;

        if (headerText == null) HeadText.color = HeaderTextColor;
        else HeadText.color = headerText;

        if (bodyText == null) GenericBodyText.color = BodyTextColor;
        else GenericBodyText.color = bodyText;

        // Backgrounds

        HeaderBackgroundImage.sprite = headerBackground;
        Background.sprite = background;
        GenericBackground.sprite = bodyBackground;

        // Extra
        this.transform.Scale(size);
        if (closeButtonCallback == null) CloseButton.onClick.AddListener(CloseCallback.Invoke);
        else CloseButton.onClick.AddListener(closeButtonCallback.Invoke);
        EnableInfoPopUp();
    }
    public void BuildOptionPopupPlainColor(string header, string body, float size, Color background, Color headerBackground, Color bodyBackground, Color headerText, Color bodyText, UnityEvent closeButtonCallback, UnityEvent cancelCallback, UnityEvent acceptCallback)
    {
        // Text
        if (header == null) HeadText.text = PopUpHeaderText;
        else HeadText.text = header;

        if (body == null) OptionBodyText.text = PopUpBodyText;
        else OptionBodyText.text = body;

        if (headerText == null) HeadText.color = HeaderTextColor;
        else HeadText.color = headerText;

        if (bodyText == null) OptionBodyText.color = BodyTextColor;
        else OptionBodyText.color = bodyText;

        // Backgrounds
        if (background == null) Background.color = PrimaryBackground;
        else Background.color = background;

        if (headerBackground == null) HeaderBackgroundImage.color = HeaderBackground;
        else HeaderBackgroundImage.color = headerBackground;

        if (bodyBackground == null) OptionBackground.color = BodyBackground;
        else OptionBackground.color = bodyBackground;

        // Extra
        this.transform.Scale(size);
        if (closeButtonCallback == null) CloseButton.onClick.AddListener(CloseCallback.Invoke);
        else CloseButton.onClick.AddListener(closeButtonCallback.Invoke);
        if (acceptCallback == null) AcceptButton.onClick.AddListener(AcceptCallback.Invoke);
        else AcceptButton.onClick.AddListener(acceptCallback.Invoke);
        if (cancelCallback == null) CancelButton.onClick.AddListener(CancelCallback.Invoke);
        else CancelButton.onClick.AddListener(cancelCallback.Invoke);
        EnableOptionPopUp();
    }

    public void BuildOptionPopupSprite(string header, string body, float size, Sprite background, Sprite headerBackground, Sprite bodyBackground, Color headerText, Color bodyText, UnityEvent closeButtonCallback, UnityEvent cancelCallback, UnityEvent acceptCallback)
    {
        // Text
        if (header == null) HeadText.text = PopUpHeaderText;
        else HeadText.text = header;

        if (body == null) OptionBodyText.text = PopUpBodyText;
        else OptionBodyText.text = body;

        if (headerText == null) HeadText.color = HeaderTextColor;
        else HeadText.color = headerText;

        if (bodyText == null) OptionBodyText.color = BodyTextColor;
        else OptionBodyText.color = bodyText;

        // Backgrounds

        HeaderBackgroundImage.sprite = headerBackground;
        Background.sprite = background;
        OptionBackground.sprite = bodyBackground;

        // Extra
        this.transform.Scale(size);
        if (closeButtonCallback == null) CloseButton.onClick.AddListener(CloseCallback.Invoke);
        else CloseButton.onClick.AddListener(closeButtonCallback.Invoke);
        if (acceptCallback == null) AcceptButton.onClick.AddListener(AcceptCallback.Invoke);
        else AcceptButton.onClick.AddListener(acceptCallback.Invoke);
        if (cancelCallback == null) CancelButton.onClick.AddListener(CancelCallback.Invoke);
        else CancelButton.onClick.AddListener(cancelCallback.Invoke);
        EnableOptionPopUp();
    }

    private void EnableGenericPopUp() 
    {
        _popupCanvasGroup.alpha = 1;
        _popupCanvasGroup.interactable = true;
        _popupCanvasGroup.blocksRaycasts = true;
    }
    private void EnableInfoPopUp() 
    {
        EnableGenericPopUp();

        OptionCanvasGroup.alpha = 0;
        OptionCanvasGroup.interactable = false;
        OptionCanvasGroup.blocksRaycasts = false;
        
        GenericCanvasGroup.alpha = 1;
        GenericCanvasGroup.interactable = true;
        GenericCanvasGroup.blocksRaycasts = true;
    }

    private void EnableOptionPopUp() 
    {
        EnableGenericPopUp();

        GenericCanvasGroup.alpha = 0;
        GenericCanvasGroup.interactable = false;
        GenericCanvasGroup.blocksRaycasts = false;

        OptionCanvasGroup.alpha = 1;
        OptionCanvasGroup.interactable = true;
        OptionCanvasGroup.blocksRaycasts = true;
    }
}
