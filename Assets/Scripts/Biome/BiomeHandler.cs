using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Xml;
using UnityEngine;

public class BiomeHandler : MonoBehaviour
{
    private readonly Dictionary<BiomeType, Biome> biomes = new();

    public Dictionary<BiomeType, List<GroundTile>> TilesPerBiome = new();

    [SerializeField] private List<BiomeType> BiomeFilters = new();

    private void Awake()
    {
        var biomeArray = Resources.LoadAll("ScriptableObjects/Biomes", typeof(Biome));
        foreach (Biome biome in biomeArray)
        {
            biomes[biome.Type] = biome;
        }
    }

    public bool AddBiomeFilter(BiomeType biome) 
    {
        if (BiomeFilters.Contains(biome)) return false;
        BiomeFilters.Add(biome);
        return true;
    }
    
    public bool RemoveBiomeFilter(BiomeType biome) 
    {
        return BiomeFilters.Remove(biome);
    }

    public List<WeightedTile> GetFilteredBiomes() 
    {
        List<WeightedTile> returnTiles = new();
        for (int i = 0; i < BiomeFilters.Count; i++) 
        {
            returnTiles.Add(biomes[BiomeFilters[i]].TilePrefab);
        }
        return returnTiles;
    }
}
