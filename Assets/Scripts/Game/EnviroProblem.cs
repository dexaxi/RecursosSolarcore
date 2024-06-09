using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnviroProblem", menuName = "RecursosSolarcore/EnviroProblem", order = 1)]
[System.Serializable]
public class EnviroProblem : ScriptableObject
{
    public new string name;
    public string Title;
    public List<string> Descriptions;
    public EnviroProblemType Type;
    public EnviroProblemSection Section;
    public List<Sprite> Sprites;
    public Sprite Icon;
    public Color color;
    public List<BiomeType> PossibleBiomes;
    public List<ActionPlan> PossibleSolutions;
    public List<EnviroConsequenceType> RelatedConsecuences;
    public List<EnviroProblemType> RelatedProblems;
}
