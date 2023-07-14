using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rider : MonoBehaviour
{
    public ElevatorDispatcher dispatcher;

    private float elevatorGroup = 0;
    private float riderSpeed = 3f;

    private int callingFloor = 0;
    private int destinationFloor = 4;

    // private RiderState riderState = RiderState.GoingToElevator;
    /*enum RiderState
    {
        GoingToElevator,
        RequestingElevator,
        WaitingForElevator,
        Riding,
        GoingToDestination
    }*/
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Elevator requestedElevator;

    private void RideOnElevator()
    {
        // this.transform.position = requestedElevator.transform.position;

        // feet of capsule is on ground of elevator
        // var yOffset = (requestedElevator.transform.localScale.y - this.transform.localScale.y) / 2;
        
        this.transform.position = new Vector3(requestedElevator.transform.position.x, requestedElevator.transform.position.y, requestedElevator.transform.position.x);
    }

    // Update is called once per frame
    void Update()
    {
        /*switch (riderState)
        {
            case RiderState.GoingToElevator:
                MoveToElevator();
                break;
            case RiderState.RequestingElevator:
                requestedElevator = dispatcher.GetBestElevator(callingFloor, destinationFloor);
                requestedElevator.ElevatorOnRequestedFloor += GetOnElevator;
                requestedElevator.AddDesiredFloor(callingFloor);
                riderState = RiderState.WaitingForElevator;
                break;
            case RiderState.Riding:
                RideOnElevator();
                break;
        }*/
    }

    private void GetOnElevator(int floor)
    {
        /*requestedElevator.ElevatorOnRequestedFloor -= GetOnElevator;
        riderState = RiderState.Riding;
        requestedElevator.ElevatorOnRequestedFloor += DestinationReached;
        
        // TODO do this in the dispatcher
        requestedElevator.AddDesiredFloor(destinationFloor);*/
    }

    private void DestinationReached(int floor)
    {
        /*if (floor != destinationFloor)
            return;
        requestedElevator.ElevatorOnRequestedFloor -= DestinationReached;
        riderState = RiderState.GoingToDestination;*/
    }

    private void MoveToElevator()
    {
        /*// TODO get elevator group to walk to
        float elevatorLocation = 0;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, elevatorLocation), riderSpeed * Time.deltaTime);
        
        if (Mathf.Abs(transform.position.z - elevatorLocation) > 0.01f)
        {
            // Smoothly interpolate the elevator's position towards the target position
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, elevatorLocation), riderSpeed * Time.deltaTime);
        }
        else
        {
            // Set the elevator's position to the exact target position
            transform.position = new Vector3(transform.position.x, transform.position.y, elevatorLocation);
            riderState = RiderState.RequestingElevator;
        }*/
    }
}
