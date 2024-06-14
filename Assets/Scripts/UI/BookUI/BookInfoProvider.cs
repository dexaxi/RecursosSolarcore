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
    public string AlterationTitle;
    public List<string> AlterationDescriptionList;
    public List<Sprite> AlterationSpritesDescriptions;
    public Sprite ProblemIcon;
    public List<EnviroConsequenceType> Consequences;
    public List<EnviroProblemType> Problems;
    public EnviroProblemProvider(EnviroProblemSection section, EnviroProblemType type, string title, 
        string alterationTitle, List<string> descriptions, List<Sprite> sprites, Sprite icon, 
        List<EnviroConsequenceType> consequences, List<EnviroProblemType> problems) 
    {
        Section = section;
        Type = type;
        Title = title;
        AlterationTitle = alterationTitle;
        AlterationDescriptionList = descriptions;
        AlterationSpritesDescriptions = sprites;
        ProblemIcon = icon;
        Consequences = consequences;
        Problems = problems;
    }
}
public class BookInfoProvider
{

    public readonly List<EnviroProblemProvider> EnviroProblems = new();
    public BiomeType BiomeType;
    public Texture2D BiomeSprite;
    public string BiomeName;
    private Biome _biome;

    public BookInfoProvider(BiomeType type, EnviroAlteration alteration) 
    {
        BiomeType = type;
        SetBiome();
        List<EnviroProblem> problems = GetFilteredProblems(alteration);
        foreach (EnviroProblem problem in problems) 
        {
                EnviroProblemProvider provider = new()
                {
                    Section = problem.Section,
                    Type = problem.Type,
                    Title = problem.Title,
                    AlterationDescriptionList = alteration.DescriptionList,
                    AlterationTitle = alteration.Title,
                    AlterationSpritesDescriptions = alteration.SpriteDescriptions,
                    ProblemIcon = problem.Icon,
                    Consequences = problem.RelatedConsecuences,
                    Problems = problem.RelatedProblems
                };
                EnviroProblems.Add(provider);
        }
    }

    private void SetBiome()
    {
        Biome biome = BiomeHandler.Instance.GetBiome(BiomeType);
        _biome = biome;
        BiomeSprite = BiomePhaseHandler.Instance.GetBiomeScreenshot(biome);
        BiomeName = _biome.name;
    }

    public string GetEnviroProblemSectionString(EnviroProblemSection section) 
    {
        switch (section) 
        {
            case EnviroProblemSection.Plantlife:
                return "Flora";
            case EnviroProblemSection.Wildlife:
                return "Fauna";
            case EnviroProblemSection.Floor:
                if (BiomeSortingRule.IsWaterBiome(_biome.Type)) return "Agua";
                return "Suelo";
            default:
                return "ERROR: INVALID DATA";
        }
    }

    public Sprite GetEnviroProblemIcon(EnviroProblemSection section) 
    {
        switch (section) 
        {
            case EnviroProblemSection.Plantlife:
                return RelationUIManager.Instance.Plantlife;
            case EnviroProblemSection.Wildlife:
                return RelationUIManager.Instance.Wildlife;
            case EnviroProblemSection.Floor:
            default:
                if (BiomeSortingRule.IsWaterBiome(_biome.Type)) return RelationUIManager.Instance.Water;
                return RelationUIManager.Instance.Floor;
        }
    }

    public List<EnviroProblem> GetFilteredProblems(EnviroAlteration alteration) 
    {
        List<EnviroProblem> problems = RelationHandler.Instance.GetFilteredProblems();
        List<EnviroProblem> returnList = new();
        foreach (EnviroProblem problem in problems)
        {
            if (alteration.EnviroProblems.Contains(problem.Type) && !returnList.Contains(problem))
            {
                returnList.Add(problem);
            }
        }
        
        return returnList;
    }

    public Dictionary<EnviroProblemProvider, List<EnviroConsequence>> GetFilteredConsequences(List<EnviroProblemProvider> problems)
    {
        Dictionary<EnviroProblemProvider, List<EnviroConsequence>> returnDict = new();
        List<EnviroConsequence> consequences = RelationHandler.Instance.GetFilteredConsequences();

        foreach (EnviroProblemProvider problem in problems)
        {
            List<EnviroConsequence> problemConsequences = new();
            foreach (EnviroConsequence consequence in consequences)
            {
                if (problem.Consequences.Contains(consequence.Type) && !problemConsequences.Contains(consequence))
                {
                    problemConsequences.Add(consequence);
                }
            }
            returnDict[problem] = problemConsequences;
        }
        return returnDict;
    }

}
