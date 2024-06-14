using Cysharp.Threading.Tasks.Triggers;
using DUJAL.Systems.Audio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPop : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { AudioManager.Instance.Play("Pop"); }); ;
    }
}
