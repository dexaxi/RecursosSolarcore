using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "RecursosSolarcore/Biome", order = 0)]
[System.Serializable]
public class Biome : ScriptableObject
{
    public new string name;
    public BiomeType Type;
    public string Description;

    public WeightedTile TilePrefab;

    [HideInInspector]
    public List<GroundTile> biomeTiles;
}
