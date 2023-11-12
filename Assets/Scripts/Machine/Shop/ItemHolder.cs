using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    private MachineShopItem _machineItem;

    public void SetMachine(Machine machine) 
    {
        _machineItem.SetMachine(machine);
    }

    public MachineShopItem GetMachineItem()
    {
        return _machineItem;
    }

}
