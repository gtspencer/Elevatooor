using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorRequest
{
    public int riderId;
    public int floor;
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