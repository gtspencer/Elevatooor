using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Leveling up unlocks new things to build, but also increases riders coming
/// </summary>
public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    public int CurrentAvarageGoldPerCustomer = 10;
    
    [SerializeField] private const int STARTING_LEVEL = 1;

    [SerializeField] private int currentLevel;

    private int totalRiders = 0;

    private int totalGold = 0;
    // Start is called before the first frame update
    void Start()
    {
        currentLevel = STARTING_LEVEL;

        EventRepository.Instance.OnRiderFinished += RiderFinished;
        EventRepository.Instance.OnMoneyMade += MoneyMade;
    }

    private void MoneyMade(int newGold)
    {
        totalGold += newGold;
    }

    private void RiderFinished(RiderV2 rider)
    {
        totalRiders++;
    }

    public void IncreaseLevel()
    {
        currentLevel++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
