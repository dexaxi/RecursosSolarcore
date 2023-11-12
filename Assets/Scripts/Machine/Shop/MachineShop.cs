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

    public void PopulateShop()
    {
        MachineHandler machineHandler = MachineHandler.Instance;

        _allFilteredMachines = machineHandler.GetFilteredMachines();
        Queue<Machine> machineQueue = new(_allFilteredMachines);

        foreach (ItemHolder itemHolder in _itemHolders) 
        {
            itemHolder.SetMachine(machineQueue.Dequeue());
        }
    }

}
