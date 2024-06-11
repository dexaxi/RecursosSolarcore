using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public enum CompletionRate
{
    Min = 65,
    Mid = 70,
    High = 75,
    Max = 80
}

public class BiomePhaseHandler : MonoBehaviour
{
    public static BiomePhaseHandler Instance { get; private set; }
    public Dictionary<BiomeType, List<EnviroProblemType>> ProblemsPerBiome = new();
    public Dictionary<EnviroProblemType, List<MachineType>> MachinesPerProblem = new();

    public Dictionary<EnviroProblemType, CompletionRate> MaxCompletion;
    public Dictionary<EnviroProblemType, int> CurrentCompletion;
    public Dictionary<BiomeType, EnviroProblem> CurrentPhasePerBiome;
    public List<BiomeType> CompletedBiomes = new();
    public Dictionary<MachineType, int> MachinePlaceRestrictionCount;

    private List<Biome> _biomes;
    private List<EnviroProblem> _problems;
    private List<Machine> _machines;

    private readonly List<CompletionRate> _completionRates = new() { CompletionRate.Min, CompletionRate.Mid, CompletionRate.Max, CompletionRate.High };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitGameplay()
    {
        _biomes = BiomeHandler.Instance.GetFilteredBiomes();
        var alterations = RelationHandler.Instance.GetFilteredAlterations();
        _problems = RelationHandler.Instance.GetFilteredProblems();
        _machines = MachineHandler.Instance.GetFilteredMachines();

        foreach (var biome in _biomes)
        {
            ProblemsPerBiome[biome.Type] = new();
            CurrentPhasePerBiome[biome.Type] = null;
            foreach (var alteration in alterations)
            {
                if (biome.EnviroAlterations.Contains(alteration.Type))
                {
                    foreach (var problem in _problems)
                    {
                        if (CurrentPhasePerBiome[biome.Type] == null) CurrentPhasePerBiome[biome.Type] = problem;
                        else if (CurrentPhasePerBiome[biome.Type].Phase > problem.Phase) CurrentPhasePerBiome[biome.Type] = problem;
                        MachinesPerProblem[problem.Type] = new();
                        foreach (var machine in _machines)
                        {
                            if (problem.PossibleSolutions.Contains(machine.Type))
                            {
                                ProblemsPerBiome[biome.Type].Add(problem.Type);
                                MachinesPerProblem[problem.Type].Add(machine.Type);
                            }
                        }
                    }
                }
            }
        }

        foreach (var problem in _problems)
        {
            MaxCompletion[problem.Type] = _completionRates[Random.Range(0, _completionRates.Count)];
            CurrentCompletion[problem.Type] = 0;
        }


    }

    public void ProcessMachineImpact(PlaceableMachine machine)
    {
        float restrictionVal = machine.GetRelatedMachine().GetOptimizationTierValue();
        var phase = GetPhaseFromMachine(machine.GetMachineType());

        if (machine.GetRelatedMachine().RestrictionType == MachineRestrictionType.Gambling)
        {
            bool passGamble = Gamble(restrictionVal);
            if (!passGamble)
            {
                AttemptResetPhase(machine.GetMachineType(), phase);
                return;
            }
        }
        else if (machine.GetRelatedMachine().RestrictionType == MachineRestrictionType.LimitedPlacing)
        {
            MachinePlaceRestrictionCount[machine.GetMachineType()]++;
            if (MachinePlaceRestrictionCount[machine.GetMachineType()] > machine.GetRelatedMachine().OptimizationLevel)
            {
                AttemptResetPhase(machine.GetMachineType(), phase);
                return;
            }
        }
        ProgressPhase(machine, phase);
    }

    public void ProgressPhase(PlaceableMachine machine, EnviroProblemType phase)
    {
        int val = 0;
        if (machine.GetRelatedMachine().PatternType == PatternType.Biome)
        {
            val = machine.GetRelatedMachine().CompletionRate;
        }
        else if (machine.GetRelatedMachine().PatternType == PatternType.Pattern)
        {
            var biomeType = GetBiomeFromPhase(phase);
            var tiles = BiomeHandler.Instance.TilesPerBiome[biomeType];
            var tileCount = tiles.Count;
            var affectedTileCount = 0;
            foreach (GroundTile tile in tiles)
            {
                var affectingMachines = tile.affectingMachines;
                var affectingTypes = tile.GetAffectingMachineTypes();
                foreach (var plaMachine in affectingMachines)
                {
                    if (plaMachine == machine && !affectingTypes.Contains(machine.GetMachineType()))
                    {
                        tile.affectingMachines.Add(machine);
                        affectedTileCount++;
                    }
                }
            }
            val = 100 * (affectedTileCount / tileCount);
        }
        CurrentCompletion[phase] += val;

        CheckPhaseCompletion(phase);
    }

    public void CheckPhaseCompletion(EnviroProblemType phase)
    {
        if (CurrentCompletion[phase] >= (int)MaxCompletion[phase])
        {
            var biome = GetBiomeFromPhase(phase);
            NextPhase(biome);
        }
    }

    public void NextPhase(BiomeType biome)
    {
        EnviroProblem curPhase = CurrentPhasePerBiome[biome];
        bool found = false;
        foreach (var problem in _problems)
        {
            if (CurrentPhasePerBiome[biome].Phase < problem.Phase && ProblemsPerBiome[biome].Contains(problem.Type))
            {
                found = true;
                CurrentPhasePerBiome[biome] = problem;
                continue;
            }
        }
        if (!found) CompletedBiomes.Add(biome);
    }

    public void CheckAllBiomesCompleted()
    {
        if (CompletedBiomes.Count == BiomeHandler.Instance.GetFilteredBiomes().Count) 
        {
            UnityEngine.Debug.Log("LEVEL FINISHED");
        }
    }

    public EnviroProblemType GetPhaseFromMachine(MachineType type)
    {
        foreach (var problem in MachinesPerProblem.Keys)
        {
            foreach (var machine in _machines)
            {
                if (machine.Type == type) return problem;
            }
        }
        return (EnviroProblemType)(-1);
    }
    
    public BiomeType GetBiomeFromPhase(EnviroProblemType type)
    {
        foreach (var biome in ProblemsPerBiome.Keys)
        {
            foreach (var problem in _problems)
            {
                if (problem.Type == type) return biome;
            }
        }
        return (BiomeType)(-1);
    }

    public void AttemptResetPhase(MachineType type, EnviroProblemType phase)
    {
        ResetPhase(type, phase);
    }
    public void ResetPhase(MachineType type, EnviroProblemType phase)
    {
        var machines = FindObjectsOfType<PlaceableMachine>().Where((PlaceableMachine x) => x.GetMachineType() == type);
        foreach (var machine in machines) 
        {
            machine.Sell();
            CurrentCompletion[phase] = 0;
        }
    }

    public bool Gamble(float gamble) 
    {
        return Random.Range(0.0f, 1.0f) > gamble;
    }
}
