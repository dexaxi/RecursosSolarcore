using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class MachineShop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button PrevButton;
    [SerializeField] private Button NextButton;


    private ItemHolder[] _itemHolders;

    private List<Machine> _allFilteredMachines = new();
    private int _currentShopIndex;

    private void Awake()
    {
        _itemHolders = GetComponentsInChildren<ItemHolder>();
        PrevButton.onClick.AddListener(PrevShopTabPressed);
        NextButton.onClick.AddListener(NextShopTabPressed);
        _currentShopIndex = 0;
    }

    private void Start()
    {
        MachineHandler.Instance.AddMachineFilter(MachineType.Type01);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type02);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type03);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type04);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type05);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type06);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type07);
        PopulateShop();
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
}
