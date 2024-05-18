using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MachineShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _priceTextObject;
    [SerializeField] private GameObject machinePrefab;
    private PlayerCurrencyManager _currencyManager;
    private Machine _machine;

    private void Awake() 
    {
        _currencyManager = FindObjectOfType<PlayerCurrencyManager>();
    }

    public void SetMachine(Machine machine) 
    {
        _machine = machine;
        _priceTextObject.text = _machine.Cost.ToString();
    }

    public Machine GetMachine() 
    {
        return _machine;
    }

    public void BuyMachine()
    {
        MachineShop.Instance.DisableShopItems();
        var popUp = GenericPopUpLoader.LoadGenericPopUp();
        UnityEvent acceptEvent = new();
        acceptEvent.AddListener(PerformBuyMachine);
        UnityEvent cancelEvent = new();
        cancelEvent.AddListener(MachineShop.Instance.EnableShopItems);
        popUp.BuildOptionPopupPlainColor("Buy " + _machine.name, "This operation will cost " + _machine.Cost, 1, new Color(155, 155, 155, 0.3f), new Color(0, 155, 155, 0.6f), new Color(0, 155, 155, 0.6f), Color.white, Color.white, acceptEvent, cancelEvent);
    }

    private void PerformBuyMachine()
    {
        float cost = _machine.Cost;
        if (_currencyManager.RemoveCurrency(cost))
        {
            InstantiateMachine();
            MachineShop.Instance.DisableShop();
        }
        else
        {
            var popUp = GenericPopUpLoader.LoadGenericPopUp();
            popUp.BuildInfoPopupPlainColor("Warning!", "You are out of funds for this purchase...", 1, new Color(155, 155, 155, 0.3f), new Color(0, 155, 155, 0.6f), new Color(0, 155, 155, 0.6f), Color.white, Color.white);
            Debug.LogWarning("WARNING: Not enough funds...");
        }
        MachineShop.Instance.EnableShopItems();
    }

    public void InstantiateMachine() 
    {
        Vector3 mousePos = CameraUtils.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos -= new Vector3(0, 0, -8.0f);
        var instance = Instantiate(machinePrefab, mousePos, Quaternion.identity);
        PlaceableMachine placeableMachine = instance.GetComponent<PlaceableMachine>();
        placeableMachine.CopyMachine(_machine);
        placeableMachine.Initialize();
    }
}
