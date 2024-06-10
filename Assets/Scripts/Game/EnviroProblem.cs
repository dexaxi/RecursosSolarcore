using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnviroProblem", menuName = "RecursosSolarcore/EnviroProblem", order = 1)]
[System.Serializable]
public class EnviroProblem : ScriptableObject
{
    public new string name;
    public string Title;
    public string Description;
    public EnviroProblemType Type;
    public EnviroProblemSection Section;
    public Sprite Icon;
    public Color color;
    public List<MachineType> PossibleSolutions;
    public List<EnviroConsequenceType> RelatedConsecuences;
    public List<EnviroProblemType> RelatedProblems;

    public class EnviroProblemDTO
    {
		public string name;
		public string Title;
		public string Description;
		public EnviroProblemType Type;
		public EnviroProblemSection Section;
		public Sprite Icon;
		public Color color;
		public List<MachineType> PossibleSolutions;
		public List<EnviroConsequenceType> RelatedConsecuences;
		public List<EnviroProblemType> RelatedProblems;
	}
}
