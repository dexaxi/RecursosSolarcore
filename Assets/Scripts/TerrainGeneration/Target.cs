
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(Highlightable))]
public class Target : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<TextMeshProUGUI> _statsMesh;

    [Header("Settings")]
    [SerializeField] TargetType _targetType;

    private readonly string _targetCode;
    public string TargetCode
    {
        get
        {
            return _targetCode;
        }
    }

    public TargetType TargetType
    {
        get
        {
            return _targetType;
        }
    }

    [SerializeField] [Range(0, 10)] int _maxHealth;

    public int CurrentHealth { get; private set; }
    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
    }

    public bool IsAlive
    {
        get
        {
            return CurrentHealth > 0;
        }
    }


    public Highlightable Highlightable { get; private set; }

    public GroundTile CurrentGroundTile { get; private set; }

    private CameraMovement _cameraMovement;


    void Awake()
    {
        Highlightable = GetComponent<Highlightable>();

        _cameraMovement = FindObjectOfType<CameraMovement>();


        var selectable = GetComponent<Selectable>();
        selectable.selectAction += DisplayStats;
        selectable.deselectAction += HideStats;

    }

    public void SetCurrentGroundTile(GroundTile tile)
    {
        CurrentGroundTile = tile;
    }

    void Start()
    {
        HideStats();
    }

    void DisplayStats()
    {
        foreach (TextMeshProUGUI tmpro in _statsMesh) 
        {
            tmpro.enabled = true;
        }
        UpdateText();
    }

    void UpdateText()
    {
            foreach (TextMeshProUGUI tmpro in _statsMesh) 
            {
                tmpro.text += $"{GetParameterName(new { CurrentHealth })}: {CurrentHealth}";
            }
    }

    void HideStats()
    {
        foreach (TextMeshProUGUI tmpro in _statsMesh)
        {
            tmpro.enabled = false;
        }
    }

    public static string GetParameterName<T>(T item) where T : class
    {
        if (item == null)
            return string.Empty;

        return typeof(T).GetProperties()[0].Name;
    }

}
