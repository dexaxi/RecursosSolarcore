using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineHandler : MonoBehaviour
{
    public static MachineHandler Instance;

    private readonly Dictionary<MachineType, Machine> _machines = new();

    public Dictionary<Vector2Int, PlaceableMachine> PlacedMachines = new();
    public Dictionary<BiomeType, MachineType> MachinesPerBiome = new();

    [SerializeField] private List<MachineType> _machineFilters = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void PopulateMachineResources() 
    {
        _machines.Clear();
        var machineArray = Resources.LoadAll("ScriptableObjects/Machines", typeof(Machine));
        var alterations = RelationHandler.Instance.GetFilteredAlterations();
        var problems = RelationHandler.Instance.GetFilteredProblems();
        foreach (Machine machine in machineArray.Cast<Machine>())
        {
            foreach (EnviroAlteration alteration in alterations) 
            {
                foreach(EnviroProblem problem in problems) 
                {
                    if (alteration.EnviroProblems.Contains(problem.Type) && problem.PossibleSolutions.Contains(machine.Type))
                    {
                        machine.CompatibleBiomes.Add(alteration.Biome);
                        _machines[machine.Type] = machine;
                        MachinesPerBiome[alteration.Biome] = machine.Type;
                    }
                }
            }
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

    public void ClearMachineFilters() 
    {
        _machineFilters.Clear();
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

    public Vector2Int FindEmptyCellCoords()
    {
        GroundTile[] tiles = FindObjectsOfType<GroundTile>();

        PlacedMachines.TryGetValue(tiles[tiles.Length/2].CellCoord, out PlaceableMachine centerPlaceable);
        if (centerPlaceable == null) return tiles[tiles.Length / 2].CellCoord;


        foreach (GroundTile tile in tiles)
        {
            PlacedMachines.TryGetValue(tile.CellCoord, out PlaceableMachine placeableMachine);
            if (placeableMachine == null) return tile.CellCoord;
        }

        return Vector2Int.zero;
    }


}
