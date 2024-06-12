using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIInfoManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI _currentMoney;

    public TextMeshProUGUI CurrentMoney
    {
        get
        {
            return _currentMoney;
        }
        set
        {
            _currentMoney = CurrentMoney;
        }
    }
    private void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }
}
