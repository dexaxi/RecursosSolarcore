using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PatternType
{
    Pattern,
    Biome
}

public enum MachineRestrictionType 
{
    LimitedPlacing,
    Gambling
}

[CreateAssetMenu(fileName = "Machine", menuName = "RecursosSolarcore/Machine", order = 1)]
[System.Serializable]
public class Machine : ScriptableObject
{
    public static float SELL_COST_MULTIPLIER = 0.8f;
    public new string name;
    public MachineType Type;
    public string title;
    public string Description;
    public float Cost;
    public Mesh MeshFilter;
    public Material MeshRenderer;
    public Sprite ShopSprite;
    public Texture2D RangePattern;
    public int OptimizationLevel;
    [HideInInspector] public List<BiomeType> CompatibleBiomes;
    [HideInInspector] public PatternType PatternType;
    public MachineRestrictionType MachineRestrictionType;
    private HighlightPattern _highlightPattern;

    public float CalculateSellCost()
    {
        return Cost * SELL_COST_MULTIPLIER;
    }

    public int[,] GetRangePattern() { return _highlightPattern.GetPattern(); }


    public Machine() 
    {
        name = "MachineXXX";
        Type = MachineType.Type01;
        Description = "Machine Uninitialized";
        Cost = -1;
        MeshFilter = null;
        MeshRenderer = null;
        ShopSprite = null;
        if (RangePattern != null)
        {
            _highlightPattern = new HighlightPattern(RangePattern);
            PatternType = PatternType.Pattern;
        }
        else 
        {
            PatternType = PatternType.Biome;
        }
        CompatibleBiomes = new List<BiomeType>();
        OptimizationLevel = -1;
    }

    public void Copy(Machine machine) 
    {
        name = machine.name;
        Type = machine.Type;
        Description = machine.Description;
        Cost = machine.Cost;
        MeshFilter = machine.MeshFilter;
        MeshRenderer = machine.MeshRenderer;
        ShopSprite = machine.ShopSprite;
        RangePattern = machine.RangePattern;
        _highlightPattern = new HighlightPattern(RangePattern);
        CompatibleBiomes = new List<BiomeType>();
        foreach (BiomeType biomeType in machine.CompatibleBiomes) 
        {
            CompatibleBiomes.Add(biomeType);
        }
        OptimizationLevel = machine.OptimizationLevel;
        PatternType = machine.PatternType;
        MachineRestrictionType = machine.MachineRestrictionType;
    }
}
