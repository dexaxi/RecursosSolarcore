using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "RecursosSolarcore/Level", order = 1)]
[System.Serializable]
public class Level : ScriptableObject
{
    public new string name;
    public string Description;

    public int LevelIndex;

    public List<EnviroProblem> EnviroProblems;

    private float _budget;



    public float CalculateBudget() 
    {
        _budget = 0;
        foreach(EnviroProblem problem in EnviroProblems) 
        {
            _budget += 1000;
        }
        return _budget;
    }

    public string BudgetToString() 
    {
        return _budget.ToString() + "€";
    }


}
