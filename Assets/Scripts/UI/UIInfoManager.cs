using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIInfoManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI _infoText;
    [SerializeField] Image _infoCurrentSprite;
    [SerializeField] Image _infoCurrentBackground;

    public TextMeshProUGUI InfoText
    {
        get
        {
            return _infoText;
        }
        set
        {
            _infoText = InfoText;
        }
    }

}
