using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class MachineShop : MonoBehaviour
{
    public static MachineShop Instance;

    [Header("References")]
    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;


    private ItemHolder[] _itemHolders;

    private CanvasGroup _canvasGroup;
    private List<Machine> _allFilteredMachines = new();
    private int _currentShopIndex;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _itemHolders = GetComponentsInChildren<ItemHolder>();
        _canvasGroup = GetComponent<CanvasGroup>();
        PrevButton.onClick.AddListener(PrevShopTabPressed);
        NextButton.onClick.AddListener(NextShopTabPressed);
        _currentShopIndex = 0;
    }

    public void PopulateShop()
    {
        MachineHandler machineHandler = MachineHandler.Instance;

        _allFilteredMachines = machineHandler.GetFilteredMachines();
        for (int i = 0; i < _itemHolders.Length; i++) 
        {
            if (i + _currentShopIndex < _allFilteredMachines.Count)
            {
                _itemHolders[i].EnableHolder();
                Machine machine = _allFilteredMachines[i + _currentShopIndex];
                Debug.Log(machine.name);
                _itemHolders[i].SetMachine(machine);
            }
            else 
            {
                _itemHolders[i].DisableHolder();
            }
        }
        HandleShopButtonsVisibility();
    }

    private void NextShopTabPressed() 
    {
        MachineHandler machineHandler = MachineHandler.Instance;
        _allFilteredMachines = machineHandler.GetFilteredMachines();
        
        if (_currentShopIndex + _itemHolders.Length >= _allFilteredMachines.Count) return;
        
        _currentShopIndex += _itemHolders.Length;
        PopulateShop();
    }

    private void PrevShopTabPressed() 
    {
        if (_currentShopIndex - _itemHolders.Length < 0) return;
        
        _currentShopIndex -= _itemHolders.Length;
        PopulateShop();
    }

    private void HandleShopButtonsVisibility() 
    {
        MachineHandler machineHandler = MachineHandler.Instance;
        _allFilteredMachines = machineHandler.GetFilteredMachines();
        if (_allFilteredMachines.Count <= _itemHolders.Length) 
        {
            PrevButton.gameObject.SetActive(false);
            NextButton.gameObject.SetActive(false);
        }
    }

    [ContextMenu("EnableShop")]
    public void EnableShop()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    [ContextMenu("DisableShop")]
    public void DisableShop()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}
