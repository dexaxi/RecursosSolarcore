using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIInfoManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI _unitName;
    [SerializeField] Image _currentSprite;
    [SerializeField] Image _currentSpriteBackground;
    [SerializeField] Sprite[] _portraitSprites;

    public TextMeshProUGUI unitName
    {
        get
        {
            return _unitName;
        }
        set
        {
            _unitName = unitName;
        }
    }

    public Target currentTarget { get; private set; }

    public void SetTargetUnit(Target target)
    {
        currentTarget = target;
        Color color;
        ColorUtility.TryParseHtmlString("#14f7ff", out color );
        _currentSpriteBackground.color = color;                

    }
}
