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
    No_Restriction,
    Limited_Placing,
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
    public MachineRestrictionType RestrictionType;
    public int RestrictionTier;
    public int CompletionRateModifier;

    private HighlightPattern _highlightPattern;
    [HideInInspector] public PatternType PatternType;
    [HideInInspector] public List<BiomeType> CompatibleBiomes;
    public float CalculateSellCost()
    {
        return Cost * SELL_COST_MULTIPLIER;
    }

    public int[,] GetRangePattern() { return _highlightPattern.GetPattern(); }

    public float GetOptimizationTierValue() 
    {
        float optimizationValue = 0.0f;
        switch (RestrictionType) 
        {
            case MachineRestrictionType.Limited_Placing:
                optimizationValue = RestrictionTier; //Veces que se puede poner
                break;
            case MachineRestrictionType.Gambling:
                if (RestrictionTier == 0) optimizationValue = 0.35f; // % de chance a que pete
                else if (RestrictionTier == 1) optimizationValue = 0.65f;
                else if (RestrictionTier == 2) optimizationValue = 0.90f; 
                else if (RestrictionTier > 2) optimizationValue = - 1.0f;
                break;
            default:
                optimizationValue = - 1.0f;
                break;
        }
        return optimizationValue;
    }

    public Machine() 
    {
        //Esto quizás se llame despues de setearlo mediante el importmarronero
        if (name != "") return;

        name = "MachineXXX";
        Description = "Machine Uninitialized";
        Cost = -1;
        MeshFilter = null;
        MeshRenderer = null;
        ShopSprite = null;
        if (RangePattern != null) { _highlightPattern = new HighlightPattern(RangePattern); }
        CompatibleBiomes = new List<BiomeType>();
        RestrictionTier = -1;
        CompletionRateModifier = -1;
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
        if (RangePattern == null)
        {
            PatternType = PatternType.Biome;
        }
        else 
        {
            _highlightPattern = new HighlightPattern(RangePattern);
            PatternType = PatternType.Pattern;
        }
        CompatibleBiomes = new List<BiomeType>();
        foreach (BiomeType biomeType in machine.CompatibleBiomes) 
        {
            CompatibleBiomes.Add(biomeType);
        }
        RestrictionTier = machine.RestrictionTier;
        CompletionRateModifier = machine.CompletionRateModifier;
        RestrictionType = machine.RestrictionType;
    }

    public class MachineDTO
    {
		public string name;
		public MachineType Type;
		public string Title;
		public string Description;
		public float Cost;
		public Mesh MeshFilter;
		public Material MeshRenderer;
		public Sprite ShopSprite;
		public Texture2D RangePattern;
		public MachineRestrictionType RestrictionType;
		public int RestrictionTier;
		public int CompletionRateModifier;
	}
}
