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
    public List<string> AlterationDescriptionList;
    public List<Sprite> AlterationSpritesDescriptions;
    public Sprite ProblemIcon;
    public List<EnviroConsequenceType> Consequences;
    public List<EnviroProblemType> Problems;
    public EnviroProblemProvider(EnviroProblemSection section, EnviroProblemType type, string title, List<string> descriptions, List<Sprite> sprites, Sprite icon, List<EnviroConsequenceType> consequences, List<EnviroProblemType> problems) 
    {
        Section = section;
        Type = type;
        Title = title;
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
    public Sprite BiomeSprite;
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
        List<Biome> biomes = BiomeHandler.Instance.GetFilteredBiomes();
        foreach (Biome biome in biomes) 
        {
            if (biome.Type == BiomeType) 
            {
                _biome = biome; 
                break; 
            }
        }
        BiomeSprite = _biome.Sprite;
        BiomeName = _biome.name;
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
