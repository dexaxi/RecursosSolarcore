using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Machine", menuName = "RecursosSolarcore/Machine", order = 1)]
[System.Serializable]
public class Machine : ScriptableObject
{
    public const float SELL_COST_MULTIPLIER = 0.8f;
    public new string name;
    public MachineType Type;
    public string Description;
    public float Cost;
    public Mesh MeshFilter;
    public Material MeshRenderer;
    public Sprite ShopSprite;
    public float CalculateSellCost()
    {
        return Cost * SELL_COST_MULTIPLIER;
    }
}
