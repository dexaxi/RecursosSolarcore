using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Machine", menuName = "RecursosSolarcore/Machine", order = 0)]
public class Machine : ScriptableObject
{
    public const float SELL_COST_MULTIPLIER = 0.8f;
    public new string name;
    public string Description;
    public float Cost;
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;

    public float CalculateSellCost()
    {
        return Cost * SELL_COST_MULTIPLIER;
    }
}
