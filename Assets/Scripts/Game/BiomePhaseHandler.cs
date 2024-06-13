using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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

    public Dictionary<EnviroProblemType, CompletionRate> MaxCompletion = new();
    public Dictionary<EnviroProblemType, int> CurrentCompletion = new();
    public Dictionary<BiomeType, EnviroProblem> CurrentPhasePerBiome = new();
    public List<BiomeType> CompletedBiomes = new();
    public Dictionary<MachineType, int> MachinePlaceRestrictionCount = new();

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
        IsUsingUI.IsInResetPhase = false;
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
                        if (alteration.EnviroProblems.Contains(problem.Type)) 
                        {
                            if (CurrentPhasePerBiome[biome.Type] == null)
                            {
                                CurrentPhasePerBiome[biome.Type] = problem;
                            }
                            else if (CurrentPhasePerBiome[biome.Type].Phase > problem.Phase) 
                            {
                                CurrentPhasePerBiome[biome.Type] = problem;
                            }
                            ProblemsPerBiome[biome.Type].Add(problem.Type);
                            MachinesPerProblem[problem.Type] = new();
                            foreach (var machine in _machines)
                            {
                                if (problem.PossibleSolutions.Contains(machine.Type))
                                {
                                    MachinesPerProblem[problem.Type].Add(machine.Type);
                                    MachinePlaceRestrictionCount[machine.Type] = 0;
                                }
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
        else if (machine.GetRelatedMachine().RestrictionType == MachineRestrictionType.Limited_Placing)
        {
            MachinePlaceRestrictionCount[machine.GetMachineType()]++;
            if (MachinePlaceRestrictionCount[machine.GetMachineType()] > machine.GetRelatedMachine().RestrictionTier)
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
            val = machine.GetRelatedMachine().CompletionRateModifier;
        }
        else if (machine.GetRelatedMachine().PatternType == PatternType.Pattern)
        {
            var biomeType = GetBiomeFromPhase(phase);
            var allMachines = MachineHandler.Instance.PlacedMachines.Values;
            var affectedTiles = new Dictionary<GroundTile, List<PlaceableMachine>>();
            var affectedTileCount = 0;
            foreach (var plaMachine in allMachines) 
            {
                var otherMachineTiles = plaMachine.GetCurrentAffectedTiles();
                foreach(var otherTile in otherMachineTiles) 
                {
                    affectedTiles[otherTile].Add(plaMachine);
                }
            }
            var tiles = machine.GetCurrentAffectedTiles();
            foreach (var tile in tiles)
            {
                bool containsType = false;
                foreach (var plaMachine in affectedTiles[tile])
                {
                    if (plaMachine.GetMachineType() == (machine.GetMachineType()))
                    {
                        containsType = true;
                    }
                }
                if (!containsType) affectedTileCount += 1;
            }
            var tileCount = BiomeHandler.Instance.TilesPerBiome[biomeType].Count;
            val = (int) (100.0f * ( affectedTileCount / (float) tileCount));
        }
        CurrentCompletion[phase] += val;

        CheckPhaseCompletion(phase);
    }

    public void CheckPhaseCompletion(EnviroProblemType phase)
    {
        if (CurrentCompletion[phase] >= (int) MaxCompletion[phase])
        {
            var biome = GetBiomeFromPhase(phase);
            NextPhase(biome);
        }
    }

    public void NextPhase(BiomeType biome)
    {
        bool found = false;
        foreach (var problem in _problems)
        {
            if (CurrentPhasePerBiome[biome].Phase < problem.Phase && ProblemsPerBiome[biome].Contains(problem.Type))
            {
                found = true;
                CurrentPhasePerBiome[biome] = problem;
                MachineShop.Instance.PopulateShop(biome);
                continue;
            }
        }
        if (!found) 
        {
            CompletedBiomes.Add(biome);
            CheckAllBiomesCompleted();
        }
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
            foreach (var machine in MachinesPerProblem[problem])
            {
                if (machine == type) return problem;
            }
        }
        return (EnviroProblemType)(-1);
    }
    
    public BiomeType GetBiomeFromPhase(EnviroProblemType type)
    {
        foreach (var biome in ProblemsPerBiome.Keys)
        {
            foreach (var problem in ProblemsPerBiome[biome])
            {
                if (problem == type) return biome;
            }
        }
        return (BiomeType)(-1);
    }

    public void AttemptResetPhase(MachineType type, EnviroProblemType phase)
    {
        var biome = BiomeHandler.Instance.GetFilteredBiomes().Where( (Biome x) => x.Type == GetBiomeFromPhase(phase) ).FirstOrDefault();
        var tiles = BiomeHandler.Instance.TilesPerBiome[biome.Type];
        int tileCount = 0;
        Vector3 tilePos = Vector3.zero;
        foreach(var tile in tiles) 
        {
            tileCount++;
            tilePos += tile.transform.position;
            tile.Highlightable.Highlight("Dead");
            tile.UpdateBiomeColor(Color.gray, Color.gray);
        }
        IsUsingUI.IsInResetPhase = true;
        Vector3 tileCenter = tilePos / tileCount;
        GameObject resetInstance = Instantiate(CompletionUIManager.Instance.ResetButton, tileCenter + new Vector3(0, 4.5f, 0), Quaternion.identity);
        var resetButton = resetInstance.GetComponentInChildren<Button>();
        resetButton.onClick.AddListener(delegate
        {
            ResetPhase(type, phase);
            IsUsingUI.IsInResetPhase = false;
            var tiles = BiomeHandler.Instance.TilesPerBiome[biome.Type];
            foreach (var tile in tiles)
            {
                tile.Highlightable.Unhighlight();
                tile.UpdateBiomeColor(tile.Biome.grassBottomColor, tile.Biome.grassTopColor);
            }
            Destroy(resetInstance.gameObject);
        });
        MachineDisplay.Instance.ExitDisplay();
        RoboDialogueManager.Instance.PlayOnce("ReiniciarFase");
    }
    public void ResetPhase(MachineType type, EnviroProblemType phase)
    {
        IsUsingUI.IsInResetPhase = false;
        var biome = GetBiomeFromPhase(phase);
        var machines = MachineHandler.Instance.PlacedMachines.Values;
        CompletionUIManager.Instance.UpdateUI(BiomeHandler.Instance.GetFilteredBiomes().Where( (Biome x) => x.Type == biome).ToList()[0]);
        foreach (var machine in machines) 
        {
            if (MachinesPerProblem[phase].Contains(machine.GetMachineType()))
            {
                machine.Sell();
            }
        }
        CurrentCompletion[phase] = 0;
        MachinePlaceRestrictionCount[type] = 0;
        
    }

    public bool Gamble(float gamble) 
    {
        return Random.Range(0.0f, 1.0f) > gamble;
    }
}
