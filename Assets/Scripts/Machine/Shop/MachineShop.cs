using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineShop : MonoBehaviour
{
    private ItemHolder[] _itemHolders;

    private List<Machine> _allFilteredMachines = new();

    private void Awake()
    {
        _itemHolders = GetComponentsInChildren<ItemHolder>();
    }

    private void Start()
    {
        MachineHandler.Instance.AddMachineFilter(MachineType.Type01);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type02);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type03);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type04);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type05);
        MachineHandler.Instance.AddMachineFilter(MachineType.Type06);
        PopulateShop();
    }

    public void PopulateShop()
    {
        MachineHandler machineHandler = MachineHandler.Instance;

        _allFilteredMachines = machineHandler.GetFilteredMachines();
        Queue<Machine> machineQueue = new(_allFilteredMachines);
        foreach (ItemHolder itemHolder in _itemHolders) 
        {
            Machine machine = machineQueue.Dequeue();
            Debug.Log(machine.name);
            itemHolder.SetMachine(machine);
        }
    }

}
