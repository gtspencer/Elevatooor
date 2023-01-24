using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorRequest
{
    public int callingFloor;
    public int destinationFloor;
    public Rider rider;
    public bool movingUp => callingFloor < destinationFloor;
}
