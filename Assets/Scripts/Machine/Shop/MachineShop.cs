using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineShop : MonoBehaviour
{
    public static MachineShop Instance;

    [Header("References")]
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button closeButton;
    [SerializeField] TextMeshProUGUI ProblemTitle;


    private ItemHolder[] _itemHolders;

    private CanvasGroup _canvasGroup;
    private List<Machine> _allFilteredMachines = new();
    private int _currentShopIndex;

    private BiomeType _currentBiome;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _itemHolders = GetComponentsInChildren<ItemHolder>();
        _canvasGroup = GetComponent<CanvasGroup>();
        prevButton.onClick.AddListener(PrevShopTabPressed);
        nextButton.onClick.AddListener(NextShopTabPressed);
        closeButton.onClick.AddListener(delegate { RelationUIManager.Instance.ShowBookButton.enabled = true; });

        _currentShopIndex = 0;
    }

    public void PopulateShop(BiomeType biome)
    {
        _currentBiome = biome;
        var machinesPerProblem = BiomePhaseHandler.Instance.MachinesPerProblem;
        var curPhase = BiomePhaseHandler.Instance.CurrentPhasePerBiome[biome];
        var possibleMachines = MachineHandler.Instance.GetFilteredMachines().Where( (Machine x) => (machinesPerProblem[curPhase.Type].Contains(x.Type))).ToList();
        var biomeName =  BiomeHandler.Instance.GetBiome(biome).name;
        var phaseName = BiomePhaseHandler.Instance.CurrentPhasePerBiome[biome].Title;
        ProblemTitle.text = biomeName + ": " + phaseName;
        for (int i = 0; i < _itemHolders.Length; i++)
        {
            if (i + _currentShopIndex < possibleMachines.Count)
            {
                _itemHolders[i].EnableHolder();
                Machine machine = possibleMachines[i + _currentShopIndex];
                Debug.Log(machine.name);
                _itemHolders[i].SetMachine(machine);
            }
            else
            {
                _itemHolders[i].DisableHolder();
            }
        }

        HandleShopButtonsVisibility();
    }

    private void NextShopTabPressed()
    {
        MachineHandler machineHandler = MachineHandler.Instance;
        _allFilteredMachines = machineHandler.GetFilteredMachines();
        
        if (_currentShopIndex + _itemHolders.Length >= _allFilteredMachines.Count) return;
        
        _currentShopIndex += _itemHolders.Length;
        PopulateShop(_currentBiome);
    }

    private void PrevShopTabPressed() 
    {
        if (_currentShopIndex - _itemHolders.Length < 0) return;
        
        _currentShopIndex -= _itemHolders.Length;
        PopulateShop(_currentBiome);
    }

    public void DisableShopItems() 
    {
        foreach (ItemHolder itemHolder in _itemHolders) 
        {
            itemHolder.GetComponent<Button>().enabled = false;
            itemHolder.GetComponent<Button>().interactable = false;
        }
        closeButton.enabled = false;
        closeButton.interactable = false;
        prevButton.enabled = false;
        prevButton.interactable = false;
        nextButton.enabled = false;
        nextButton.interactable = false;
    }
    
    public void EnableShopItems() 
    {
        foreach (ItemHolder itemHolder in _itemHolders) 
        {
            itemHolder.GetComponent<Button>().enabled = true;
            itemHolder.GetComponent<Button>().interactable = true;
        }
        closeButton.enabled = true;
        closeButton.interactable = true;
        prevButton.enabled = true;
        prevButton.interactable = true;
        nextButton.enabled = true;
        nextButton.interactable = true;
    }

    private void HandleShopButtonsVisibility() 
    {
        MachineHandler machineHandler = MachineHandler.Instance;
        _allFilteredMachines = machineHandler.GetFilteredMachines();
        if (_allFilteredMachines.Count <= _itemHolders.Length) 
        {
            prevButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }
    }

    public void EnableShop()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        IsUsingUI.IsUsingShop = true;
    }

    public void DisableShop()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        IsUsingUI.IsUsingShop = false;
    }

}
