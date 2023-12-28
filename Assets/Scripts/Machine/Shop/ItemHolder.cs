using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
    private MachineShopItem _machineItem;
    private Image _image;
    private Button _button;
    private void Awake()
    {
        _machineItem = GetComponentInChildren<MachineShopItem>();
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    public void SetMachine(Machine machine) 
    {
        _machineItem.SetMachine(machine);
        _image.sprite = _machineItem.GetMachine().ShopSprite;
        _button.onClick.AddListener(_machineItem.BuyMachine);
    }

    public MachineShopItem GetMachineItem()
    {
        return _machineItem;
    }

}
