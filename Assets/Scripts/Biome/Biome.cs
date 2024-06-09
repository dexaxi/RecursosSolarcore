using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "RecursosSolarcore/Biome", order = 0)]
[System.Serializable]
public class Biome : ScriptableObject
{
    [Header("Text")]
    public new string name;
    public BiomeType Type;
    public string Description;

    [Header("Rendering")]
    public Mesh Mesh;
    public Material Material;

    [Header("Logic")]
    public GameObject tilePrefab;
    public int biomeWeight;

    [HideInInspector]
    public List<GroundTile> biomeTiles;

    [HideInInspector]
    public int spawnCount;

    [HideInInspector]
    public Sprite Sprite;

    public void StartBiome() 
    {
        spawnCount = 0;
    }
}
