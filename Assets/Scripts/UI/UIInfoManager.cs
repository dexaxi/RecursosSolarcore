using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIInfoManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI _currentMoney;
    [SerializeField] Image _infoCurrentSprite;
    [SerializeField] Image _infoCurrentBackground;

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

}
