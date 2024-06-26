using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCurrencyManager : MonoBehaviour
{
    public static PlayerCurrencyManager Instance;

    private UIInfoManager _infoManager; 
    public float Currency { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _infoManager = FindObjectOfType<UIInfoManager>();
    }

    public void AddCurrency(float amount) 
    {
        if (amount < 0) 
        {
            amount = Mathf.Abs(amount);
            Debug.LogWarning("WARNING: trying to add negative amount to currency.");
        }
        Currency += amount;
        UpdateCurrencyUI();
        _infoManager.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public bool RemoveCurrency(float amount) 
    {
        if (amount < 0)
        {
            amount = Mathf.Abs(amount);
            Debug.LogWarning("WARNING: amount has to be positive.");
        }
        if (Currency - amount < 0) return false;
        Currency -= amount;
        UpdateCurrencyUI();
        return true;
    }

    private void UpdateCurrencyUI()
    {
        _infoManager.CurrentMoney.text = Currency.ToString();
    }

    public void SetCurrency(float amount) { Currency = amount; UpdateCurrencyUI(); }
}
