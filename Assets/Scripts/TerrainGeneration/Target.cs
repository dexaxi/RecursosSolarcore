
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

    private string _targetCode;
    public string targetCode
    {
        get
        {
            return _targetCode;
        }
    }

    public TargetType targetType
    {
        get
        {
            return _targetType;
        }
    }

    [SerializeField] [Range(0, 10)] int _maxHealth;

    public int currentHealth { get; private set; }
    public int maxHealth
    {
        get
        {
            return _maxHealth;
        }
    }

    public bool isAlive
    {
        get
        {
            return currentHealth > 0;
        }
    }


    public Highlightable highlightable { get; private set; }

    public GroundTile currentGroundTile { get; private set; }

    CameraMovement _cameraMovement;


    void Awake()
    {
        highlightable = GetComponent<Highlightable>();

        _cameraMovement = FindObjectOfType<CameraMovement>();


        var selectable = GetComponent<Selectable>();
        selectable.selectAction += DisplayStats;
        selectable.deselectAction += HideStats;

    }

    public void SetCurrentGroundTile(GroundTile tile)
    {
        currentGroundTile = tile;
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
                tmpro.text += $"{GetParameterName(new { currentHealth })}: {currentHealth}";
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
