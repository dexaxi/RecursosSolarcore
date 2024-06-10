using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SearchService;
using UnityEngine.UI;

public class MachineDisplay : MonoBehaviour
{
    public static MachineDisplay Instance;
    private PlaceableMachine _machine;

    [Header("Canvas Group References")]
    [SerializeField] CanvasGroup    SellUI;
    [SerializeField] CanvasGroup    PlaceUI;
    [SerializeField] CanvasGroup    MoveUI;
    [SerializeField] CanvasGroup    ConfirmUI;

    [Header("Button References")]
    [SerializeField] Button     moveButton;
    [SerializeField] Button     sellButton;
    [SerializeField] Button     exitButton;
    [SerializeField] Button     confirmPlacement;
    [SerializeField] Button     cancelPlacement;
    [SerializeField] Button     canvelMove;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        HandleButtonEvents();
    }

    private void HandleButtonEvents() 
    {
        moveButton.onClick.AddListener(ShowMoveDisplay);
        sellButton.onClick.AddListener(SellMachine);
        exitButton.onClick.AddListener(ExitDisplay);
        confirmPlacement.onClick.AddListener(ConfirmPlacement);
        cancelPlacement.onClick.AddListener(CancelPlacement);
        canvelMove.onClick.AddListener(CancelMove);
    }

    public void SetMachine(PlaceableMachine machine)
    {
        _machine?.UnHighlightRange();
        _machine = machine;
    }

    public void SellMachine()
    {
        _machine.Sell();
        ExitDisplay();
    }
    public void MoveMachine()
    {
        _machine.Move();
    }

    public void ConfirmPlacement()
    {
        if (_machine.ConfirmPlacement()) 
        {
            ExitDisplay();
        }
    }

    public void CancelPlacement()
    {
        _machine.SellFullCost();
        ExitDisplay();
    }

    public void CancelMove()
    {
        _machine.CancelMove(_machine.PrevCoords);
    }

    public void ExitDisplay() 
    {
        DisableSellUI();
        DisableMoveUI();
        DisablePlaceUI();
        _machine.UnHighlightRange();
        MachineShop.Instance.EnableShopButton();
        Selectable.UnlockSelectable();
    }

    public void ShowMoveDisplay() 
    {
        DisableSellUI();
        DisablePlaceUI();
        EnableMoveUI();
        MachineShop.Instance.DisableShopButton();
        MoveMachine();
    }
    
    public void ShowPlaceDisplay() 
    {
        DisableMoveUI();
        DisableSellUI();
        EnablePlaceUI();
        MachineShop.Instance.DisableShopButton();
        MoveMachine();
    }

    public void ShowSellDisplay() 
    {
        DisableMoveUI();
        DisablePlaceUI();
        MachineShop.Instance.DisableShopButton();
        EnableSellUI();
    }

    private void EnableSellUI() { SellUI.alpha = 1; SellUI.interactable = true; SellUI.blocksRaycasts = true; }
    private void DisableSellUI() { SellUI.alpha = 0; SellUI.interactable = false; SellUI.blocksRaycasts = false; }
    
    private void EnablePlaceUI() { PlaceUI.alpha = 1; PlaceUI.interactable = true; PlaceUI.blocksRaycasts = true; EnableConfirmUI(); }
    private void DisablePlaceUI() { PlaceUI.alpha = 0; PlaceUI.interactable = false; PlaceUI.blocksRaycasts = false; DisableConfirmUI(); }
    
    private void EnableMoveUI() { MoveUI.alpha = 1; MoveUI.interactable = true; MoveUI.blocksRaycasts = true; EnableConfirmUI();  }
    private void DisableMoveUI() { MoveUI.alpha = 0; MoveUI.interactable = false; MoveUI.blocksRaycasts = false; DisableConfirmUI(); }

    private void EnableConfirmUI() { ConfirmUI.alpha = 1; ConfirmUI.interactable = true; ConfirmUI.blocksRaycasts = true; }
    private void DisableConfirmUI() { ConfirmUI.alpha = 0; ConfirmUI.interactable = false; ConfirmUI.blocksRaycasts = false; }
}
