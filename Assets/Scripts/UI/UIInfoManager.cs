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

    public TextMeshProUGUI UnitName
    {
        get
        {
            return _unitName;
        }
        set
        {
            _unitName = UnitName;
        }
    }

    public Target CurrentTarget { get; private set; }

    public void SetTargetUnit(Target target)
    {
        CurrentTarget = target;
        ColorUtility.TryParseHtmlString("#14f7ff", out Color color);
        _currentSpriteBackground.color = color;
    }
}
