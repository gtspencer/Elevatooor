using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    [SerializeField] private int currentGold = 0;

    public int CurrentGold
    {
        get => currentGold;
        set
        {
            currentGold = value;
            UpdateGoldUI();
        }
    }

    [SerializeField] private Text goldTextUI;
    private void UpdateGoldUI()
    {
        goldTextUI.text = CurrentGold.ToString();
    }

    public int CurrentAvarageGoldPerCustomer { get; set; } = 10;

    public void AddGoldFromCustomer(float goldMultiplier)
    {
        AddGold((int)Mathf.Floor(CurrentAvarageGoldPerCustomer * goldMultiplier));
    }
    
    public void AddGold(int newGold)
    {
        this.CurrentGold = newGold;
    }

    public void RemoveGold(int oldGold)
    {
        this.CurrentGold = oldGold;
    }
}
