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

    public void TestAddGold(int newGold)
    {
        EventRepository.Instance.OnMoneyMade.Invoke(newGold);
        this.CurrentGold += newGold;
    }

    public void AddGoldFromCustomer(float goldMultiplier)
    {
        var variance = PlayerStatsManager.Instance.CurrentAvarageGoldPerCustomer * .25;
        var baseGold = Random.Range((int) (PlayerStatsManager.Instance.CurrentAvarageGoldPerCustomer - variance), (int) (PlayerStatsManager.Instance.CurrentAvarageGoldPerCustomer + variance) + 1);
        AddGold((int)Mathf.Floor(baseGold * goldMultiplier));
    }
    
    private void AddGold(int newGold)
    {
        EventRepository.Instance.OnMoneyMade.Invoke(newGold);
        this.CurrentGold += newGold;
    }

    public void RemoveGold(int oldGold)
    {
        this.CurrentGold -= oldGold;
    }
}
