using DUJAL.Systems.Audio;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField] Camera biomeScreenshotCamera;

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
            var tiles = machine.GetCurrentAffectedTiles();
            foreach (var plaMachine in allMachines) 
            {
                if (plaMachine == machine) continue;
                var otherMachineTiles = plaMachine.GetCurrentAffectedTiles();
                foreach(var otherTile in otherMachineTiles) 
                {
                    if (!affectedTiles.ContainsKey(otherTile)) affectedTiles[otherTile] = new();
                    affectedTiles[otherTile].Add(plaMachine);
                }
            }
            foreach (var tile in tiles)
            {
                bool containsType = false;
                if (affectedTiles.TryGetValue(tile, out List<PlaceableMachine> plaMachinesInTile)) 
                {
                    foreach (var plaMachine in plaMachinesInTile)
                    {
                        if (plaMachine.GetMachineType() == (machine.GetMachineType()))
                        {
                            containsType = true;
                        }
                    }
                }
                if (!containsType) affectedTileCount += 1;
            }
            var tileCount = BiomeHandler.Instance.TilesPerBiome[biomeType].Count;
            val = (int) (100.0f * ( affectedTileCount / (float) tileCount));
        }
        CurrentCompletion[phase] += val;
        machine.CurrentAffectingPhasePercent = val;
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
            if (ProblemsPerBiome[biome].Contains(problem.Type) && (CurrentPhasePerBiome[biome].Phase + 1) == problem.Phase)
            {
                found = true;
                CurrentPhasePerBiome[biome] = problem;
                MachineShop.Instance.PopulateShop(biome);
                break;
            }
        }
        if (!found) 
        {
            DBConnection.Instance.phase_sucess++;
            DBConnection.Instance.UpdateUser();
            CompletedBiomes.Add(biome);
            if (!CheckAllBiomesCompleted())
            {
                RoboDialogueManager.Instance.StartRoboDialogue("BiomeRestorationCompleted");
                var biomes = ProblemsPerBiome.Keys;
                foreach (var newBiome in biomes)
                {
                    if (!CompletedBiomes.Contains(newBiome))
                    {
                        MachineShop.Instance.PopulateShop(newBiome);
                        CompletionUIManager.Instance.UpdateUI(BiomeHandler.Instance.GetBiome(newBiome));
                        return;
                    }
                }
            }
            else
            {
                var popUp = GenericPopUpLoader.LoadGenericPopUp();
                UnityEvent finishDemoEvent = new();
                finishDemoEvent.AddListener(delegate { SceneLoader.Instance.LoadScene(SceneIndex.MAIN_MENU); });
                popUp.BuildInfoPopupPlainColor("�Demo Completada!", "�Enhorabuena! Has completado la demo de EcoRescue�, much�simas gracias por jugar :)", 1, Color.white, Color.white, Color.white, Color.black, Color.black, finishDemoEvent);
            }
        }
    }

    public bool CheckAllBiomesCompleted()
    {
        if (CompletedBiomes.Count == BiomeHandler.Instance.GetFilteredBiomes().Count) 
        {
            UnityEngine.Debug.Log("LEVEL FINISHED");
            return true;
        }
        return false;
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
        var biome = BiomeHandler.Instance.GetBiome(GetBiomeFromPhase(phase));
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
        MachineShop.Instance.DisableShopItems();
        MachineDisplay.Instance.ExitDisplay();
        RoboDialogueManager.Instance.PlayOnce("ReiniciarFase");
        AudioManager.Instance.Play("Fail");
    }
    public void ResetPhase(MachineType type, EnviroProblemType phase)
    {
        DBConnection.Instance.phase_fail++;
        IsUsingUI.IsInResetPhase = false;
        var biomeType = GetBiomeFromPhase(phase);
        var machines = MachineHandler.Instance.PlacedMachines.Values;
        CompletionUIManager.Instance.UpdateUI(BiomeHandler.Instance.GetBiome(biomeType));
        Queue<PlaceableMachine> machinesToSell = new();
        foreach (var machine in machines) 
        {
            if (MachinesPerProblem[phase].Contains(machine.GetMachineType()))
            {
                machinesToSell.Enqueue(machine);
            }
        }
        while (machinesToSell.Count > 0) 
        {
            var machine = machinesToSell.Dequeue();
            machine.Sell();
        }        
    }

    public bool Gamble(float gamble) 
    {
        return Random.Range(0.0f, 1.0f) > gamble;
    }

    public Texture2D GetBiomeScreenshot(Biome biome)
    {
        RenderTexture targetTexture = new RenderTexture(399, 271, 24);

        biomeScreenshotCamera.targetTexture = targetTexture;
        Ground.Instance.DisableOtherBiomes(biome.Type);
        biomeScreenshotCamera.Render();
        RenderTexture.active = targetTexture;
        Texture2D tex = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGBAHalf, false);
        tex.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        Ground.Instance.EnableBiomes(biome.Type);

        return tex;
    }
}
