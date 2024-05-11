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
    }

    public void DisableButton() 
    {
        _button.interactable = false;
    }

    public void EnableButton() 
    {
        _button.interactable = true;
    }

}
