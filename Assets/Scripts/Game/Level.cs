using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "RecursosSolarcore/Level", order = 1)]
[System.Serializable]
public class Level : ScriptableObject
{
    public new string name;
    public string Description;

    public int LevelIndex;

    public List<EnviroProblem> EnviroProblems;
    public List<ActionPlan> SelectedSolutions;
    public List<BiomeType> SelectedBiomes;

    [Header("Biome Sorting Settings")]
    public SortingRuleType RuleType;
    [Tooltip("Choose what channels should have water. On a 4 biome level, 1 & 2 would mean that 0 and 4 should have land biomes. Compatible with Default and Random Respect Water modes. IMPORTANT: The amount of channels should be <= the amount of water biome types (Ocean, River...).")]
    public List<int> WaterChannels;
    [Tooltip("This Dictionary is only useful for ForceBiomePositions.")]
    [SerializeField] public SerializableDictionary <int, BiomeType> ForceBiomePositions;

    private float _budget;

    public float CalculateBudget() 
    {
        _budget = 0;
        foreach(EnviroProblem problem in EnviroProblems) 
        {
            _budget += 1000;
        }
        return _budget;
    }

    public string BudgetToString() 
    {
        return _budget.ToString() + "€";
    }

    public List<Biome> HandleSortingRule() 
    {
        switch (RuleType) 
        {
            case SortingRuleType.Default:
            default:
                return BiomeHandler.Instance.SortBiomesDefault(BiomeHandler.Instance.GetFilteredBiomes(), WaterChannels);
            case SortingRuleType.Random:
                return BiomeHandler.Instance.SortBiomesRandom(BiomeHandler.Instance.GetFilteredBiomes());
            case SortingRuleType.RandomRespectWater:
                return BiomeHandler.Instance.SortBiomesRandomRespectWater(BiomeHandler.Instance.GetFilteredBiomes(), WaterChannels);
            case SortingRuleType.Forced:
                return BiomeHandler.Instance.SortBiomesForceOrder(BiomeHandler.Instance.GetFilteredBiomes(), ForceBiomePositions);
        }
    }

    public void InitLevel() 
    {
        foreach (EnviroProblem problem in EnviroProblems)
        {
            foreach (BiomeType biome in SelectedBiomes)
            {
                if (problem.PossibleBiomes.Contains(biome)) BiomeHandler.Instance.AddBiomeFilter(biome);
            }
        }

        foreach (ActionPlan plan in SelectedSolutions) 
        {
            foreach(MachineType machine in plan.PossibleMachines) 
            {
                MachineHandler.Instance.AddMachineFilter(machine);
            }
        }
        
        MachineShop.Instance.PopulateShop();
        
        Ground.Instance.StartMapGeneration();
    }
}
