using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum NodeType 
{
    NoType = -1,
    EnviroProblem_Problem = 0,
    EnviroProblem_Consequence = 1,
    EnviroConsequence_Problem = 2
}

public class AnchorPoint : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [field: SerializeField] public NodeType NodeType { get; private set; }

    public static Dictionary<BiomeType, Dictionary<EnviroProblemType, List<EnviroConsequenceType>>> ExistingProbConsRelationships = new();
    public static Dictionary<BiomeType, Dictionary<EnviroConsequenceType, List<EnviroProblemType>>> ExistingConsProbRelationships = new();
    public static Dictionary<BiomeType, Dictionary<EnviroProblemType, List<EnviroProblemType>>> ExistingProblemRelationships = new();

    public static Dictionary<BiomeType, List<Vector3[]>> LinePositions = new();
    public static Dictionary<BiomeType, List<AnchorPoint>> AnchorPoints = new();

    public static Dictionary<BiomeType, int> AllPossibleRelationsCount = new();
    public static Dictionary<BiomeType, int> RelationCount = new();

    public static Dictionary<BiomeType, UnityEvent> BiomeFinished = new();
    public static UnityEvent AllBiomesFinished = new();

    private LineSpawner _spawner;
    private RelationUIManager _relationUIManager;
    private RectTransform _rect;

    private EnviroProblemType _problemType;
    private EnviroConsequenceType _consequenceType;

    public List<EnviroProblemType> RelatedProblems;
    public List<EnviroConsequenceType> RelatedConsequences;

    private bool _performDraw;
    private Color _prevColor;

    public BiomeType CurrentBiome;

    private void Awake()
    {
        _spawner = GetComponentInParent<LineSpawner>();
        _rect = GetComponent<RectTransform>();
        _relationUIManager = GetComponentInParent<RelationUIManager>();

        ExistingConsProbRelationships.Clear();
        ExistingProbConsRelationships.Clear();
        ExistingProblemRelationships.Clear();
        LinePositions.Clear();
        AnchorPoints.Clear();
        AllPossibleRelationsCount.Clear();
        RelationCount.Clear();
        BiomeFinished.Clear();

        AllBiomesFinished.RemoveAllListeners();
    }

    public NodeType GetNodeType() { return NodeType; }

    public void UpdateBiome(BiomeType biome) 
    {
        CurrentBiome = biome;
    }

    public void SetDataType(int dataType, BiomeType biome)
    {
        CurrentBiome = biome;
        if(!RelationCount.ContainsKey(biome)) RelationCount[biome] = 0;
        if(!AllPossibleRelationsCount.ContainsKey(biome)) AllPossibleRelationsCount[biome] = 0;
        if (!ExistingConsProbRelationships.ContainsKey(biome)) ExistingConsProbRelationships[biome] = new();
        if (!ExistingProbConsRelationships.ContainsKey(biome)) ExistingProbConsRelationships[biome] = new();
        if (!ExistingProblemRelationships.ContainsKey(biome)) ExistingProblemRelationships[biome] = new();
        if (!LinePositions.ContainsKey(biome)) LinePositions[biome] = new();
        if (!AnchorPoints.ContainsKey(biome)) AnchorPoints[biome] = new();
        if (!BiomeFinished.ContainsKey(biome)) BiomeFinished[biome] = new();

        switch (GetNodeType()) 
        {
            case NodeType.EnviroConsequence_Problem:
                _consequenceType = (EnviroConsequenceType) dataType;
                break;
            case NodeType.EnviroProblem_Problem:
            case NodeType.EnviroProblem_Consequence:
                _problemType = (EnviroProblemType) dataType;
                break;
            case NodeType.NoType:
            default:
                Debug.LogError($"Trying to assign Data Type: {dataType} to invalid Node Type {GetNodeType()}.");
                break;
        }
    }

    public int GetDataType() 
    {
        switch (GetNodeType())
        {
            case NodeType.EnviroConsequence_Problem:
                return (int) _consequenceType;
            case NodeType.EnviroProblem_Problem:
            case NodeType.EnviroProblem_Consequence:
                return (int) _problemType ;
            case NodeType.NoType:
            default:
                Debug.LogError($"Trying to get Data Type from invalid Node type: {NodeType}.");
                return -1;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        _prevColor = GetComponentInChildren<Image>().color;
        GetComponentInChildren<Image>().color = ShopPopupCanvasUiElements.greenColor;
        DrawLine();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopDraw();
    }

    private void DrawLine() 
    {
        LineRenderer line = _spawner.SpawnLine(transform.position, this);
        _performDraw = true;
        DrawLoop(line).Forget();
    }

    private void StopDraw() { _performDraw = false; }

    private async UniTask DrawLoop(LineRenderer line) 
    {
        await UniTask.Delay(0);
        
        if (!_performDraw) 
        {
            HandleMouseUp(line, GetScreenMousePos());
            return;
        }
        Vector3 mousePos = GetMousePositionWorldPos();
        Vector3 anchorPos = GetAnchorWorldPos();
        line.positionCount = 2;
        line.SetPosition(0, anchorPos);
        line.SetPosition(1, mousePos);
        DrawLoop(line).Forget();
    }
    
    public void RestoreLines()
    {
        foreach (var posArr in LinePositions[CurrentBiome])
        {
            LineRenderer line = _spawner.SpawnLine(transform.position, this);
            line.positionCount = 2;
            line.SetPosition(0, posArr[0]);
            line.SetPosition(1, posArr[1]);
        }
        foreach (var anchor in AnchorPoints[CurrentBiome]) 
        {
            anchor.GetComponentInChildren<Image>().color = ShopPopupCanvasUiElements.greenColor;
        }
    }

    private Vector3 GetAnchorWorldPos() 
    {
       return new Vector3(_rect.position.x, _rect.position.y, transform.position.z - 1);
    }
    private Vector3 GetScreenMousePos() 
    {
        return new(Input.mousePosition.x, Input.mousePosition.y, CameraUtils.MainCamera.WorldToScreenPoint(transform.position).z);
    }

    private Vector3 GetMousePositionWorldPos() 
    {
        Vector3 screenPos = GetScreenMousePos();
        Vector3 pos = CameraUtils.MainCamera.ScreenToWorldPoint(screenPos);
        return pos;
    }

    private void HandleMouseUp(LineRenderer line, Vector3 mousePos)
    {
        List<AnchorPoint> results = _relationUIManager.RaycastNodes(mousePos);
        if (results.Count > 1) { Debug.LogError("ERROR: Trying to link more than 2 nodes"); HandleIncorrectNodeLink(line); return; }
        if (results.Count == 0) { HandleIncorrectNodeLink(line); return; }
        AnchorPoint destNode = results[0];
        if (!CheckRelationValid(destNode)) { HandleIncorrectNodeLink(line); return; }
        if (!UpdateExistingRelationships(destNode)) { HandleIncorrectNodeLink(line); return; }

        RelationCount[CurrentBiome]++;
        var currLinePos = new Vector3[]
        {
            line.GetPosition(0), line.GetPosition(1)
        };

        LinePositions[CurrentBiome].Add(currLinePos);
        AnchorPoints[CurrentBiome].Add(this);
        AnchorPoints[CurrentBiome].Add(destNode);

        if (RelationCount[CurrentBiome] == AllPossibleRelationsCount[CurrentBiome])
        {
            Debug.Log("FINISHED");
            if (CheckAllBiomesFinished())
            {
                AllBiomesFinished.Invoke();
                Debug.Log("ALL BIOMES FINISHED");
            }
            else BiomeFinished[CurrentBiome].Invoke();
        }
        destNode.GetComponentInChildren<Image>().color = ShopPopupCanvasUiElements.greenColor;
    }

    public bool CheckAllBiomesFinished() 
    {
        var biomes = BiomeHandler.Instance.GetFilteredBiomes();
        foreach (var biome in biomes) 
        {
            if (!RelationCount.ContainsKey(biome.Type) || RelationCount[biome.Type] != AllPossibleRelationsCount[biome.Type]) return false;
        }
        return true;
    }


private bool UpdateExistingRelationships(AnchorPoint dest) 
    {
        bool res = false;
        EnviroProblemType problemType;
        EnviroProblemType problemType2;
        EnviroConsequenceType consequenceType;

        int localData = GetDataType();
        int destData = dest.GetDataType();

        switch (GetNodeType()) 
        {
            case NodeType.EnviroConsequence_Problem:
                consequenceType = (EnviroConsequenceType) localData;
                problemType = (EnviroProblemType) destData;
                res = UpdateProbConsDictionaries(consequenceType, problemType);
                break;
            case NodeType.EnviroProblem_Consequence:
                consequenceType = (EnviroConsequenceType)destData;
                problemType = (EnviroProblemType)localData;
                res = UpdateProbConsDictionaries(consequenceType, problemType);
                break;
            case NodeType.EnviroProblem_Problem:
                problemType = (EnviroProblemType) localData;
                problemType2 = (EnviroProblemType) destData;
                res =UpdateProbProbDictionary(problemType, problemType2);
                break;
        }

        return res;
    }

    private bool UpdateProbConsDictionaries(EnviroConsequenceType consequenceType, EnviroProblemType problemType) 
    {
        if (ExistingConsProbRelationships[CurrentBiome].ContainsKey(consequenceType) && ExistingConsProbRelationships[CurrentBiome][consequenceType].Contains(problemType)) return false;
        if (ExistingProbConsRelationships[CurrentBiome].ContainsKey(problemType) && ExistingProbConsRelationships[CurrentBiome][problemType].Contains(consequenceType)) return false;
        if (!ExistingConsProbRelationships[CurrentBiome].ContainsKey(consequenceType)) ExistingConsProbRelationships[CurrentBiome][consequenceType] = new();
        if (!ExistingProbConsRelationships[CurrentBiome].ContainsKey(problemType)) ExistingProbConsRelationships[CurrentBiome][problemType] = new();
        ExistingConsProbRelationships[CurrentBiome][consequenceType].Add(problemType);
        ExistingProbConsRelationships[CurrentBiome][problemType].Add(consequenceType);
        return true;   
    }

    private bool UpdateProbProbDictionary(EnviroProblemType problemType, EnviroProblemType problemType2) 
    {
        if (ExistingProblemRelationships[CurrentBiome].ContainsKey(problemType) && ExistingProblemRelationships[CurrentBiome][problemType].Contains(problemType2)) return false;
        if (ExistingProblemRelationships[CurrentBiome].ContainsKey(problemType2) && ExistingProblemRelationships[CurrentBiome][problemType2].Contains(problemType)) return false;
        if (!ExistingProblemRelationships[CurrentBiome].ContainsKey(problemType)) ExistingProblemRelationships[CurrentBiome][problemType] = new();
        if (!ExistingProblemRelationships[CurrentBiome].ContainsKey(problemType2)) ExistingProblemRelationships[CurrentBiome][problemType2] = new();
        ExistingProblemRelationships[CurrentBiome][problemType].Add(problemType2);
        ExistingProblemRelationships[CurrentBiome][problemType2].Add(problemType);
        return true;   
    }

    private bool CheckRelationValid(AnchorPoint dest) 
    {
        bool res;
        bool isValidRelationType;
        if (IsProblem(GetNodeType()))
        {
            isValidRelationType = CheckProblemRelation(dest);
            res = CheckProblemRelationCorrect(dest);
        }
        else 
        {
            isValidRelationType = CheckConsequenceRelation(dest);
            res = CheckConsequenceRelationCorrect(dest);
        }

        if (!isValidRelationType) { res = false; return res; }

        return res;
    }

    private bool CheckProblemRelation(AnchorPoint dest) 
    {
        bool res;
        switch (GetNodeType())
        {
            case NodeType.EnviroProblem_Consequence:
                    res = dest.GetNodeType() == NodeType.EnviroConsequence_Problem;
                break;
            case NodeType.EnviroProblem_Problem:
                    res = dest.GetNodeType() == NodeType.EnviroProblem_Problem;
                break;
            default:
                res = false;
                break;
        }
        return res;
    }
    
    private bool CheckConsequenceRelation(AnchorPoint dest) 
    {
        bool res;
        switch (GetNodeType())
        {
            case NodeType.EnviroConsequence_Problem:
                res = dest.GetNodeType() == NodeType.EnviroProblem_Consequence;
                break;
            default:
                res = false;
                break;
        }
        return res;
    }

    private bool CheckProblemRelationCorrect(AnchorPoint dest)
    {
        bool res = false;
        if (GetNodeType() == NodeType.EnviroProblem_Problem)
        {
            res = RelatedProblems.Contains((EnviroProblemType)dest.GetDataType()) || dest.RelatedProblems.Contains(_problemType);
        }
        else if (GetNodeType() == NodeType.EnviroProblem_Consequence)
        {
            res = RelatedConsequences.Contains((EnviroConsequenceType)dest.GetDataType());
        }
        return res;
    }

    private bool CheckConsequenceRelationCorrect(AnchorPoint dest)
    {
        bool res;
        res = RelatedProblems.Contains((EnviroProblemType)dest.GetDataType());
        return res;
    }


    public bool IsProblem(NodeType nodeType) 
    {
        return nodeType == NodeType.EnviroProblem_Problem || nodeType == NodeType.EnviroProblem_Consequence;
    }

    private void HandleIncorrectNodeLink(LineRenderer line) 
    {
        Destroy(line.gameObject);
        GetComponentInChildren<Image>().color = _prevColor;
    }
}
