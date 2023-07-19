using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventRepository : MonoBehaviour
{
    public static EventRepository Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    public Action<ElevatorV2> OnElevatorSelected = (elevator) => { };
    public Action OnElevatorUnSelected = () => { };
    public Action<RiderV2> OnRiderFinished = (riderId) => { };
    public Action<int> OnMoneyMade = (newMoney) => { };
    public Action<int> OnHourChange = (hour) => { };
}
