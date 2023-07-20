using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class ElevatorUpgrades
{
    public static Dictionary<int, Upgrade> ElevatorSpeedUpgrades = new Dictionary<int, Upgrade>()
    {
        { 1, new Upgrade(0, 5) },
        { 2, new Upgrade(100, 7) },
        { 3, new Upgrade(500, 10) }
    };
    
    public static Dictionary<int, Upgrade> ElevatorAccelerationUpgrades = new Dictionary<int, Upgrade>()
    {
        { 1, new Upgrade(0, 5) },
        { 2, new Upgrade(100, 7) },
        { 3, new Upgrade(500, 10) }
    };
    
    public static Dictionary<int, Upgrade> ElevatorWeightLimitUpgrades = new Dictionary<int, Upgrade>()
    {
        { 1, new Upgrade(0, 500) },
        { 2, new Upgrade(100, 700) },
        { 3, new Upgrade(500, 1000) }
    };
}

public class Upgrade
{
    public int cost;
    public int value;

    public Upgrade(int cost, int value)
    {
        this.cost = cost;
        this.value = value;
    }
}
