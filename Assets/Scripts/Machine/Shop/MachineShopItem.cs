using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MachineShopItem : MonoBehaviour
{
    private Machine _machine;
    [SerializeField] private TextMeshProUGUI _priceTextObject;

    private void Awake()
    {
        _priceTextObject.text = _machine.Cost.ToString();
    }

    public void SetMachine(Machine machine) 
    {
        _machine = machine;
    }
    
    public Machine GetMachine() 
    {
        return _machine;
    }
}
