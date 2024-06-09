using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RelationHandler : MonoBehaviour
{
    public Button DEBUG_NEXTLEVEL;

    public static RelationHandler Instance { get; private set; }

    private readonly Dictionary<EnviroProblemType, EnviroProblem> _problems = new();
    private readonly Dictionary<EnviroConsequenceType, EnviroConsequence> _consequences= new();

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
        DontDestroyOnLoad(gameObject);

        DEBUG_NEXTLEVEL.onClick.AddListener(LoadNextLevel);
    }

    private void Start()
    {
        ResourceGame.Instance.ProcessActiveScene();
    }

    private void LoadNextLevel() 
    {
        SceneLoader.Instance.LoadScene(SceneIndex.LEVEL_SCENE, 1000);
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
            _consequences[consequence.Type] = consequence;
        }
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
    }

    public List<EnviroProblem> GetFilteredProblems()
    {
        List<EnviroProblem> returnProblems = new();
        for (int i = 0; i < _problemFilters.Count; i++)
        {
            returnProblems.Add(_problems[_problemFilters[i]]);
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

    public void InitLevel(BiomeType type)
    {
        List<EnviroProblem> allFilteredProblems = GetFilteredProblems();
        List<EnviroProblem> biomeFitlteredProblems = new();
        foreach (EnviroProblem problem in allFilteredProblems) 
        {
            if (problem.PossibleBiomes.Contains(type)) biomeFitlteredProblems.Add(problem);
        }
        if (biomeFitlteredProblems.Count > 3) Debug.LogError("ERROR: TOO MANY PROBLEMS PER BIOME"); 
        BookInfoProvider provider = new(type, biomeFitlteredProblems);
        BookUIManager.Instance.StartBook(provider);
        RelationUIManager.Instance.DisplayBook();
    }
}
