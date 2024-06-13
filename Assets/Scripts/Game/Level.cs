using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "RecursosSolarcore/Level", order = 1)]
[System.Serializable]
public class Level : ScriptableObject
{
    public new string name;
    public string Description;

    public int LevelIndex;

    public List<BiomeType> SelectedBiomes;

    [Header("Biome Sorting Settings")]
    [Header("Default: will use the order you've set in Selected Biomes. Compatible with Water Channels\n"
        + "Random: will randomize all of the biomes.\n"
        + "Random: Respect Water will randomize all of the biomes except whatever is explicitly indicated in the Water Channels\n"
        + "Forced: Biome Position will force the biome order according to the ForceBiomePositions dictionary.")]
    [Space(10)]
    public SortingRuleType RuleType;
    
    [Space(20)]
    [Header("Water Channels")]
    [Header("Choose what channels should have water. On a 4 biome level, 1 & 2 would mean that 0 and 4 should have land biomes."
        + "Compatible with Default and Random Respect Water modes. IMPORTANT: The amount of channels should be <= the amount of " +
        "water biome types (Ocean, River...).")]
    [Space(10)]
    public List<int> WaterChannels;
    
    [Header("Forced Biome Positions")]
    [Header("This Dictionary is only useful for ForceBiomePositions. Key is index, value is what biome goes in that index. " +
        "IMPORTANT: There can only be as many positions a biomes.")]
    [Space(20)]
    [SerializeField] public SerializableDictionary <int, BiomeType> ForceBiomePositions;
    
    private float _budget;
    [HideInInspector] public BiomeType CurrentRelationBiome;

    public float CalculateBudget() 
    {
        _budget = 0;
        foreach(EnviroAlterationType alteration in SelectedBiomes) 
        {
            _budget += 100000;
        }
        return _budget;
    }

    public string BudgetToString() 
    {
        return _budget.ToString() + "�";
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

    public void InitPreLevel() 
    {
        BiomeHandler.Instance.PopulateBiomeResources();
        GenerateBiomeFilters();

        RelationHandler.Instance.PopulateAlterations();
        GenrateAlterationFilters();
        
        RelationHandler.Instance.PopulateProblems();
        GenerateProblemFilters();
        
        RelationHandler.Instance.PopulateConsequences();
        GenerateConsequenceFilters();
        
        MachineHandler.Instance.PopulateMachineResources();
        GenerateMachineFilters();

        Ground.Instance.StartMapGeneration();

        RoboDialogueManager.Instance.StartRoboDialogue("TestLevelIntroDialogueGraph");
        AnchorPoint.AllBiomesFinished.AddListener(RelationHandler.Instance.EndRelationPhase);
    }

    public void InitBubblePhase()
    {
        IsUsingUI.IsInBubblePhase = true;
        RelationHandler.Instance.SpawnBiomeBubbles();
    }

    public void InitRelationLevel() 
    {
        RelationHandler.Instance.InitBookUI(CurrentRelationBiome);
    }

    public void InitGameplayLevel() 
    {
        BiomePhaseHandler.Instance.InitGameplay();
        var startingBiome = BiomeHandler.Instance.GetFilteredBiomes().FirstOrDefault();
        MachineShop.Instance.PopulateShop(startingBiome.Type);
        MachineShop.Instance.EnableShop();

        PlayerCurrencyManager.Instance.AddCurrency(CalculateBudget());
        RelationUIManager.Instance.EnableBookButton();

        CompletionUIManager.Instance.ShowCompletionBar(startingBiome);
    }

    private void GenerateMachineFilters() 
    {
        var enviroProblems = RelationHandler.Instance.GetFilteredProblems();
        foreach (var problem in enviroProblems) 
        {
            foreach (MachineType machine in problem.PossibleSolutions)
            {
                MachineHandler.Instance.AddMachineFilter(machine);
            }
        }
    }

    private void GenerateBiomeFilters() 
    {
        foreach (BiomeType biome in SelectedBiomes)
        {
            BiomeHandler.Instance.AddBiomeFilter(biome);
        }
    }
    private void GenrateAlterationFilters() 
    {
        var biomes = BiomeHandler.Instance.GetFilteredBiomes();
        foreach (var biome in biomes)
        {
            var count = biome.EnviroAlterations.Count;
            var random = Random.Range(0, count);
            RelationHandler.Instance.AddAlterationFilter(biome.EnviroAlterations[random]);
        }
    }

    private void GenerateProblemFilters() 
    {
        List<EnviroAlteration> filteredAlterations = RelationHandler.Instance.GetFilteredAlterations();
        foreach (EnviroAlteration enviroAlteration in filteredAlterations)
        {
            foreach (EnviroProblemType problem in enviroAlteration.EnviroProblems)
            {
                RelationHandler.Instance.AddProblemFilter(problem);
            }
        }
    }

    private void GenerateConsequenceFilters() 
    {
        List<EnviroProblem> filteredProblems = RelationHandler.Instance.GetFilteredProblems();
        foreach (EnviroProblem problem in filteredProblems) 
        {
            foreach (EnviroConsequenceType consequence in problem.RelatedConsecuences) 
            {
                RelationHandler.Instance.AddConsequenceFilter(consequence);
            }
        }
    }
}
