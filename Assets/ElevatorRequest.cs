using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorRequest
{
    public int callingFloor;
    public int destinationFloor;
    public int riderIds;
    public bool goingUp => callingFloor < destinationFloor;
}
