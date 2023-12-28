using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachineShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _priceTextObject;
    [SerializeField] private GameObject machinePrefab;
    [SerializeField] private GameObject popupPrefab;
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
        float cost = _machine.Cost;
        if (_currencyManager.RemoveCurrency(cost))
        {
            InstantiateMachine();
        }
        else 
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
            var instance = Instantiate(popupPrefab, center, Quaternion.identity, canvas.transform);
            GenericPopUp popUp = instance.GetComponent<GenericPopUp>();
            popUp.BuildInfoPopupPlainColor("Warning!", "You are out of funds for this purchase...", 2, Color.blue, Color.cyan, Color.cyan, Color.white, Color.white, null);
            Debug.LogWarning("WARNING: Not enough funds...");
        }
    }

    public void InstantiateMachine() 
    {
        Vector3 mousePos = CameraUtils.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos -= new Vector3(0, 0, -8.0f);
        var instance = Instantiate(machinePrefab, mousePos, Quaternion.identity);
        Vector2Int cellCoords = Ground.Instance.FindEmptyCellCoords();
        instance.transform.position = new Vector3(cellCoords.x, 0, cellCoords.y);
    }
}
