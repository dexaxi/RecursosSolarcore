using DUJAL.Systems.Audio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BiomeHandler : MonoBehaviour
{
    public static BiomeHandler Instance;

    private readonly Dictionary<BiomeType, Biome> _biomes = new();

    public Dictionary<BiomeType, List<GroundTile>> TilesPerBiome = new();

    [SerializeField] private List<BiomeType> _biomeFilters = new();

    public Biome GetBiome(BiomeType biomeType) 
    {
        return _biomes[biomeType];
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        PopulateBiomeResources();
    }
    public void PopulateBiomeResources() 
    {
        _biomes.Clear();
        TilesPerBiome.Clear();

        var biomeArray = Resources.LoadAll("ScriptableObjects/Biomes", typeof(Biome));
        foreach (Biome biome in biomeArray.Cast<Biome>())
        {
            biome.StartBiome();
            _biomes[biome.Type] = biome;
            TilesPerBiome[biome.Type] = new List<GroundTile>();
        }
    }

    public bool AddBiomeFilter(BiomeType biome) 
    {
        if (_biomeFilters.Contains(biome)) return false;
        _biomeFilters.Add(biome);
        return true;
    }
    
    public bool RemoveBiomeFilter(BiomeType biome) 
    {
        return _biomeFilters.Remove(biome);
    }

    public void ClearBiomeFilters()
    {
        _biomeFilters.Clear();
    }

    public List<Biome> GetFilteredBiomes()
    {
        List<Biome> returnBiomes = new();
        for (int i = 0; i < _biomeFilters.Count; i++)
        {
            returnBiomes.Add(_biomes[_biomeFilters[i]]);
        }
        return returnBiomes;
    }

    public List<Biome> SortBiomesRandom(List<Biome> biomes) 
    {
        BiomeSortingRule rule = new()
        {
            SortingRule = SortingRuleType.Random
        };
        return rule.SortBiomes(biomes);
    }

    public List<Biome> SortBiomesRandomRespectWater(List<Biome> biomes, List<int> waterChannels) 
    {
        BiomeSortingRule rule = new()
        {
            SortingRule = SortingRuleType.RandomRespectWater,
            WaterChannels = waterChannels
        };
        return rule.SortBiomes(biomes);
    }

    public List<Biome> SortBiomesDefault(List<Biome> biomes, List<int> waterChannels = null) 
    {
        BiomeSortingRule rule = new()
        {
            SortingRule = SortingRuleType.Default,
            WaterChannels = waterChannels
        };
        return rule.SortBiomes(biomes);
    }
    
    public List<Biome> SortBiomesForceOrder(List<Biome> biomes, SerializableDictionary<int, BiomeType> biomeForcePositions) 
    {
        BiomeSortingRule rule = new()
        {
            SortingRule = SortingRuleType.Forced,
            BiomeForcePositions = biomeForcePositions
        };
        return rule.SortBiomes(biomes);
    
    }
    public void PlayBiomeAudio(BiomeType biome) 
    {
        AudioManager.Instance.Play(GetBiome(biome).name);
    }
}