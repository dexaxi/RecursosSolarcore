using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public enum SortingRuleType 
{
    // OVERRIDE EVERYTHING. EMBRACE RANDOMNESS
    Random,
    // OVERRRIDE EVERYTHING, EMBRACE RANDOMNESS, RESPECTWATER
    RandomRespectWater,
    // Specify Order
    Forced,
    // Will leave the order the same UNLESS water channels are specified, then the order will remain BUT letting water channels appear where indicated
    Default
}

public class BiomeSortingRule 
{
    // In what channel/s will there be water
    public List<int> WaterChannels = new();

    public SortingRuleType SortingRule;

    public Dictionary<int, BiomeType> BiomeForcePositions = new();

    public List<Biome> SortBiomes(List<Biome> biomes)
    {
        if (SortingRule == SortingRuleType.Random)
        {
            return DoRandomSort(biomes);
        }
        else if (SortingRule == SortingRuleType.RandomRespectWater && WaterChannels.Count != 0)
        {
            return DoWaterRandomSort(biomes);
        }
        else if (SortingRule == SortingRuleType.Default)
        {
            if (WaterChannels.Count != 0)
            {
                return SetWaterChannelsAtPosition(biomes);
            }
        }
        else if (SortingRule == SortingRuleType.Forced) 
        {
            if (BiomeForcePositions.Count == biomes.Count)
            {
                return BiomeForceOrder(biomes);
            }
        }
        return biomes;
    }

    public List<Biome> BiomeForceOrder(List<Biome> biomes) 
    {
        List<Biome> returnBiomes = new();
        for (int i = 0; i < biomes.Count; i++)
        {
            returnBiomes.Add(GetBiomeFromType(biomes, BiomeForcePositions[i]));
        }
        return returnBiomes;
    }

    public Biome GetBiomeFromType(List<Biome> biomes, BiomeType type)
    {
        Debug.Log("biomes.count: " + biomes.Count);
        foreach(Biome biome in biomes) 
        {
            Debug.Log("biome.Type: " + biome.Type.ToString());
            Debug.Log("type: " + type.ToString());
            if (biome.Type == type) return biome;
        }
        Debug.LogWarning("BIOME NOT FOUND");
        return null;
    }

    public List<Biome> DoRandomSort(List<Biome> biomes) 
    {
        return Shuffle(biomes);
    }

    public List<Biome> SetWaterChannelsAtPosition(List<Biome> biomes)
    {
        Queue<Biome> waterBiomes = new();
        Queue<Biome> otherBiomes = new();
        List<Biome> biomeCopy = new();
        for (int i = 0; i < biomes.Count; i++) 
        {
            biomeCopy.Add(null);
        }

        for (int i = 0; i < biomes.Count; i++) 
        {
            if (IsWaterBiome(biomes[i].Type))
            {
                waterBiomes.Enqueue(biomes[i]);
            }
            else 
            {
                otherBiomes.Enqueue(biomes[i]);
            }
        }
        for (int i = 0; i < biomes.Count; i++) 
        {
            if (WaterChannels.Contains(i)) biomeCopy[i] = waterBiomes.Dequeue();
            else biomeCopy[i] = otherBiomes.Dequeue();
        }
        return biomeCopy;
    }

    public bool IsWaterBiome(BiomeType biomeType) 
    {
        return biomeType == BiomeType.Water;
    }

    public List<Biome> DoWaterRandomSort(List<Biome> biomes) 
    {
        List<Biome> biomeCopy = new();
        foreach (Biome biome in biomes) biomeCopy.Add(biome);
        biomeCopy = DoRandomSort(biomeCopy);
        if (WaterChannels.Count > biomes.Count)
        {
            Debug.LogWarning("WARNING: USED MORE WATER CHANNELS THAN BIOMES, DOING RANDOM SORT");
            return biomeCopy;
        }
        return SetWaterChannelsAtPosition(biomeCopy);
    }

    public static List<Biome> Shuffle(List<Biome> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            (list[n], list[k]) = (list[k], list[n]);
        }
        return list;
    }

}
