using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoManagerProvider : MonoBehaviour
{
    [Header("References")]
    [SerializeField]UIInfoManager _handUIInfo;
    [SerializeField]UIInfoManager _desktopUIInfo;

    public UIInfoManager InfoManager
    {
        get
        {
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                return _handUIInfo;
            }
            return _desktopUIInfo;
        }
    }

    private void Awake()
    {
    }
}
