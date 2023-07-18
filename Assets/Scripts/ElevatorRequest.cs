using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ElevatorRequest
{
    [SerializeField]
    public string riderId;
    [SerializeField]
    public int floor;
    [SerializeField]
    public bool goingUp;
    /*public OutsideRequest outsideRequest;
    public InsideRequest insideRequest;*/
}

public class OutsideRequest
{
    public int floor;
    public bool goingUp;
}

public class InsideRequest
{
    public int floor;
}