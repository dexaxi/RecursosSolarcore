using Cysharp.Threading.Tasks;
using DUJAL.IndependentComponents.Floater;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RelationHandler : MonoBehaviour
{
    public static RelationHandler Instance { get; private set; }

    private readonly Dictionary<EnviroAlterationType, EnviroAlteration> _alterations= new();
    private readonly Dictionary<EnviroProblemType, EnviroProblem> _problems = new();
    private readonly Dictionary<EnviroConsequenceType, EnviroConsequence> _consequences = new();

    [SerializeField] private GameObject BiomeBubble;
    [SerializeField] private List<EnviroAlterationType> _alterationFilters = new();
    [SerializeField] private List<EnviroProblemType> _problemFilters = new();
    [SerializeField] private List<EnviroConsequenceType> _consequenceFilters = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void EndRelationPhase() 
    {
        FindObjectOfType<BiomeBubble>().HideThisBubble(false);
        ResourceGame.Instance.ProcessActiveScene(LevelSceneFlow.Gameplay);
        IsUsingUI.IsInBubblePhase = false;
    }

    public void PopulateAlterations()
    {
        _alterations.Clear();
        var problemArray = Resources.LoadAll("ScriptableObjects/EnviroAlterations", typeof(EnviroAlteration));
        var biomes = BiomeHandler.Instance.GetFilteredBiomes();
        foreach (EnviroAlteration alteration in problemArray.Cast<EnviroAlteration>())
        {
            foreach (Biome biome in biomes)
            {
                if (biome.EnviroAlterations.Contains(alteration.Type))
                {
                    alteration.Biome = biome.Type;
                    continue;
                }
            }
            _alterations[alteration.Type] = alteration;
        }
    }
    
    public void PopulateProblems()
    {
        _problems.Clear();
        var problemArray = Resources.LoadAll("ScriptableObjects/EnviroProblems", typeof(EnviroProblem));
        foreach (EnviroProblem problem in problemArray.Cast<EnviroProblem>())
        {
            _problems[problem.Type] = problem;
        }
    }

    public void PopulateConsequences()
    {
        _consequences.Clear();
        var consequenceArray = Resources.LoadAll("ScriptableObjects/EnviroConsequences", typeof(EnviroConsequence));
        foreach (EnviroConsequence consequence in consequenceArray.Cast<EnviroConsequence>())
        {
            consequence.RelatedProblems.Clear();
            foreach (EnviroProblem problem in _problems.Values)
            {
                if (problem.RelatedConsecuences.Contains(consequence.Type))
                {
                    consequence.RelatedProblems.Add(problem.Type);
                }
            }
            _consequences[consequence.Type] = consequence;
        }
    }

    public void SpawnBiomeBubbles() 
    {
        var biomeTiles = BiomeHandler.Instance.TilesPerBiome;
        foreach (var biome in biomeTiles.Keys) 
        {
            Vector3 sumPoint = Vector3.zero;
            var count = 0;
            foreach(var tile in biomeTiles[biome]) 
            {
                sumPoint += tile.transform.position;
                count++;
            }
            Vector3 finalPos = count > 0 ? sumPoint / count : Vector3.zero;
            if (finalPos != Vector3.zero) 
            {
                Vector3 adjustedPos = finalPos;
                var bubble = Instantiate(BiomeBubble, adjustedPos, Quaternion.identity).GetComponent<BiomeBubble>();
                bubble.SetBiomeType(biome);
                bubble.GetComponentInChildren<FloaterComponent>().SetOffset(new Vector3(0, 1.5f, 0));
            }
        }
    }

    public void KillBiomeBubbles() 
    {
        var bubbles = FindObjectsOfType<BiomeBubble>();
        foreach (var bubble in bubbles) { Destroy(bubble.gameObject); }
    }

    public void SetBubblePhase()
    {
        IsUsingUI.IsInPrephase = false;
        ResourceGame.Instance.ProcessActiveScene(LevelSceneFlow.ShowBiomeBubbles);
    }

    public bool AddAlterationFilter(EnviroAlterationType alteration)
    {
        if (_alterationFilters.Contains(alteration)) return false;
        _alterationFilters.Add(alteration);
        return true;
    }

    public bool AddProblemFilter(EnviroProblemType problem)
    {
        if (_problemFilters.Contains(problem)) return false;
        _problemFilters.Add(problem);
        return true;
    }

    public bool AddConsequenceFilter(EnviroConsequenceType consequence)
    {
        if (_consequenceFilters.Contains(consequence)) return false;
        _consequenceFilters.Add(consequence);
        return true;
    }
    public bool RemoveProblemFilter(EnviroProblemType problem)
    {
        return _problemFilters.Remove(problem);
    }

    public bool RemoveConsequenceFilter(EnviroConsequenceType consequence)
    {
        return _consequenceFilters.Remove(consequence);
    }

    public void ClearFilters()
    {
        _consequenceFilters.Clear();
        _problemFilters.Clear();
        _alterationFilters.Clear();
    }

    public List<EnviroAlteration> GetFilteredAlterations()
    {
        List<EnviroAlteration> returnAlterations = new();
        for (int i = 0; i < _alterationFilters.Count; i++)
        {
            returnAlterations.Add(_alterations[_alterationFilters[i]]);
        }
        return returnAlterations;
    }

    public List<EnviroProblem> GetFilteredProblems()
    {
        List<EnviroProblem> returnProblems = new();
        for (int i = 0; i < _problemFilters.Count; i++)
        {
            if (_problems.TryGetValue(_problemFilters[i], out EnviroProblem problem))
            {
                returnProblems.Add(problem);
            }
        }
        return returnProblems;
    }
    
    public List<EnviroConsequence> GetFilteredConsequences()
    {
        List<EnviroConsequence> returnConsequences = new();
        for (int i = 0; i < _consequenceFilters.Count; i++)
        {
            returnConsequences.Add(_consequences[_consequenceFilters[i]]);
        }
        return returnConsequences;
    }
    
    public void InitBookUI(BiomeType type)
    {
        BookInfoProvider provider = GenerateBookInfoProvider(type);
        BookUIManager.Instance.StartBook(provider);
        RelationUIManager.Instance.StartPaper(provider);
        RelationUIManager.Instance.DisplayBook();
        AnchorPoint.AllPossibleRelationsCount[type] = RelationUIManager.Instance.CalculateAllPossibleRelations();
    }

    public BookInfoProvider GenerateBookInfoProvider(BiomeType type) 
    {
        List<EnviroAlteration> allAvailableAlterations = GetFilteredAlterations();
        EnviroAlteration generatedAlteration = null;
        foreach (EnviroAlteration alteration in allAvailableAlterations)
        {
            if (alteration.Biome == (type))
            {
                generatedAlteration = alteration;
                if (alteration.EnviroProblems.Count > 3) Debug.LogError("ERROR: TOO MANY PROBLEMS PER ALTERATION");
            }
        }
        return new BookInfoProvider(type, generatedAlteration);
    }
}
