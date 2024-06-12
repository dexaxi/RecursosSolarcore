using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnviroConsecuence", menuName = "RecursosSolarcore/EnviroConsecuence", order = 1)]
[System.Serializable]
public class EnviroConsequence : ScriptableObject
{
    public new string name;
    public string Title;
    public string Description;
    public EnviroConsequenceType Type;
    public Sprite Sprite;
    public Color color;
    [HideInInspector] public readonly List<EnviroProblemType> RelatedProblems = new();

	public class EnviroConsequenceDTO
	{
		public string name;
		public string Title;
		public string Description;
		public EnviroConsequenceType Type;
		public Sprite Sprite;
		public Color color;
	}
}
