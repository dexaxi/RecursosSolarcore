using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    private Button _button;
    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(MachineShop.Instance.EnableShop);
        _button.onClick.AddListener(DisableButton);
        _button.onClick.AddListener(delegate { RelationUIManager.Instance.ShowBookButton.enabled = false; });
    }

    public void DisableButton() 
    {
        _button.interactable = false;
    }

    public void EnableButton() 
    {
        _button.interactable = true;
    }

    public void HideButton()
    {
        var canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }
    public void ShowButton() 
    {
        var canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }
}
