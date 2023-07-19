using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class ElevatorUpgrades
{
    public static Dictionary<int, float> ElevatorSpeedUpgrades = new Dictionary<int, float>()
    {
        { 1, 5 },
        { 2, 7 },
        { 3, 10 }
    };
    
    public static Dictionary<int, float> ElevatorAccelerationUpgrades = new Dictionary<int, float>()
    {
        { 1, 5 },
        { 2, 7 },
        { 3, 10 }
    };
    
    public static Dictionary<int, float> ElevatorWeightLimitUpgrades = new Dictionary<int, float>()
    {
        { 1, 500 },
        { 2, 700 },
        { 3, 1000 }
    };
}
