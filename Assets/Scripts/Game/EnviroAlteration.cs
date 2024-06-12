using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnviroAlteration", menuName = "RecursosSolarcore/EnviroAlteration", order = 1)]
[System.Serializable]
public class EnviroAlteration : ScriptableObject
{
    public new string name;
    public string Title;
    public List<string> DescriptionList;
    public EnviroAlterationType Type;
    public List<Sprite> SpriteDescriptions;
    public Sprite Icon;
    public Color color;
    [HideInInspector] public BiomeType Biome;
    public List<EnviroProblemType> EnviroProblems;

    public class EnviroAlterationDTO
    {
        public string name;
        public string Title;
        public List<string> DescriptionList;
        public EnviroAlterationType Type;
        public List<Sprite> SpriteDescriptions;
        public Sprite Icon;
        public Color color;
        public List<EnviroProblemType> EnviroProblems;

    }
}
