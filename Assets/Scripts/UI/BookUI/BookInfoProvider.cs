using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum EnviroProblemSection 
{
    Floor = 0,
    Wildlife,
    Plantlife,
}

public struct EnviroProblemProvider
{
    public EnviroProblemSection Section;
    public EnviroProblemType Type;
    public string Title;
    public string Description;
    public Sprite Sprite;
    public Sprite Icon;
    public List<EnviroConsequenceType> Consequences;
    public EnviroProblemProvider(EnviroProblemSection section, EnviroProblemType type, string title, string description, Sprite sprite, Sprite icon, List<EnviroConsequenceType> consequences) 
    {
        Section = section;
        Type = type;
        Title = title;
        Description = description;
        Sprite = sprite;
        Icon = icon;
        Consequences = consequences;
    }
}
public class BookInfoProvider
{

    public readonly List<EnviroProblemProvider> EnviroProblems = new();
    public BiomeType BiomeType;
    public Sprite BiomeSprite;
    private Biome _biome;

    public BookInfoProvider(BiomeType type, List<EnviroProblem> problems) 
    {
        BiomeType = type;
        SetBiome();
        foreach(EnviroProblem problem in problems) 
        {
            EnviroProblemProvider provider = new()
            {
                Section = problem.Section,
                Type = problem.Type,
                Title = problem.name,
                Description = problem.Description,
                Sprite = problem.Sprite,
                Icon = problem.Icon,
                Consequences = problem.RelatedConsecuences,
            };
            EnviroProblems.Add(provider);
        }
    }

    private void SetBiome()
    {
        List<Biome> biomes = BiomeHandler.Instance.GetFilteredBiomes();
        foreach (Biome biome in biomes) { if (biome.Type == BiomeType) _biome = biome; break; }
        BiomeSprite = _biome.Sprite;
    }

    public string GetEnviroProblemSectionString(EnviroProblemSection section) 
    {
        return section switch
        {
            EnviroProblemSection.Plantlife => "Plant Life",
            EnviroProblemSection.Wildlife => "Wild Life",
            EnviroProblemSection.Floor => "Soil",
            _ => "ERROR: INVALID DATA",
        };
    }
}
