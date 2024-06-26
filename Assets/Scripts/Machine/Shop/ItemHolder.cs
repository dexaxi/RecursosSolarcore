using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] MachineShopItem _machineItem;
    [SerializeField] private Image _image;
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetMachine(Machine machine) 
    {
        _machineItem.SetMachine(machine);
        _image.sprite = _machineItem.GetMachine().ShopSprite;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(_machineItem.BuyMachine);
    }

    public MachineShopItem GetMachineItem()
    {
        return _machineItem;
    }

    public void DisableHolder() 
    {
        _button.gameObject.SetActive(false);
        _image.gameObject.SetActive(false);
        _machineItem.gameObject.SetActive(false);
    }
    
    public void EnableHolder() 
    {
        _button.gameObject.SetActive(true);
        _image.gameObject.SetActive(true);
        _machineItem.gameObject.SetActive(true);
    }
}
