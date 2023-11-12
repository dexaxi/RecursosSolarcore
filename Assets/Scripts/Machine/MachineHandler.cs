using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineHandler : MonoBehaviour
{
    public static MachineHandler Instance;

    private readonly Dictionary<MachineType, Machine> _machines = new();

    [SerializeField] private List<MachineType> _machineFilters = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        var machineArray = Resources.LoadAll("ScriptableObjects/Machines", typeof(Machine));
        foreach (Machine machine in machineArray.Cast<Machine>())
        {
            _machines[machine.Type] = machine;
        }
    }

    public bool AddMachineFilter(MachineType machine)
    {
        if (_machineFilters.Contains(machine)) return false;
        _machineFilters.Add(machine);
        return true;
    }

    public bool RemoveMachineFilter(MachineType machine)
    {
        return _machineFilters.Remove(machine);
    }

    public List<Machine> GetFilteredMachines()
    {
        List<Machine> returnMachines = new();
        for (int i = 0; i < _machineFilters.Count; i++)
        {
            returnMachines.Add(_machines[_machineFilters[i]]);
        }
        return returnMachines;
    }

}
