using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFloor : MonoBehaviour
{
    [SerializeField] private ArrowIndicators arrowIndicators;
    [SerializeField] private ElevatorV2 elevator;

    private int floor;

    private List<RiderV2> riderQueue = new List<RiderV2>();
    private Dictionary<string, ElevatorRequest> ridersToRequests = new Dictionary<string, ElevatorRequest>();

    private void Start()
    {
        arrowIndicators = GetComponentInChildren<ArrowIndicators>();
    }
    
    public void InitFloor(int floor, ElevatorV2 elevator)
    {
        this.floor = floor;
        this.elevator = elevator;
        
        elevator.AddFloorReachedCallback(floor, OnFloorStopped);
    }

    private void OnFloorStopped()
    {
        StartCoroutine(ProcessRiderQueue());
    }
    
    public void RequestRide(RiderV2 rider, bool goingUp)
    {
        ElevatorRequest newRequest = new ElevatorRequest()
        {
            floor = floor,
            riderId = rider.riderId,
            goingUp = goingUp
        };

        if (goingUp)
        {
            arrowIndicators.ToggleHighlightUp(true);
        }
        else
        {
            arrowIndicators.ToggleHighlightDown(true);
        }

        riderQueue.Add(rider);
        ridersToRequests.Add(rider.riderId, newRequest);
        
        elevator.RequestRide(newRequest);
    }

    private IEnumerator ProcessRiderQueue()
    {
        if (riderQueue.Count <= 0)
        {
            SetElevatorReadyToDepart();
            yield break;
        }

        var tempRiderQueue = new List<RiderV2>(riderQueue);
        
        bool elevatorHasNoPlan = false;
        bool goingUp = false;
        // elevator has no plan, so plan becomes first rider
        if (elevator.NextFloor == -1)
        {
            elevatorHasNoPlan = true;
            
            var firstRider = tempRiderQueue[0];

            goingUp = ridersToRequests[firstRider.riderId].goingUp;
        }
        
        // elevator going up
        if (elevator.NextFloor > elevator.CurrentFloor || (elevatorHasNoPlan && goingUp))
        {
            List<RiderV2> removeRiders = new List<RiderV2>();
            foreach (RiderV2 rider in tempRiderQueue)
            {
                if (ridersToRequests[rider.riderId].goingUp)
                {
                    rider.GetOnElevator(elevator);
                    removeRiders.Add(rider);
                    ridersToRequests.Remove(rider.riderId);
                    
                    yield return new WaitForSeconds(rider.riderSpeed / 5);
                }
            }
            
            foreach (RiderV2 removeRider in removeRiders)
            {
                riderQueue.Remove(removeRider);
            }
            
            arrowIndicators.ToggleHighlightUp(false);
        }
        // elevator going down
        else if (elevator.NextFloor < elevator.CurrentFloor || (elevatorHasNoPlan && !goingUp))
        {
            List<RiderV2> removeRiders = new List<RiderV2>();
            foreach (RiderV2 rider in tempRiderQueue)
            {
                if (!ridersToRequests[rider.riderId].goingUp)
                {
                    rider.GetOnElevator(elevator);
                    removeRiders.Add(rider);
                    ridersToRequests.Remove(rider.riderId);
                    
                    yield return new WaitForSeconds(rider.riderSpeed / 5);
                }
            }
            
            foreach (RiderV2 removeRider in removeRiders)
            {
                riderQueue.Remove(removeRider);
            }
            
            arrowIndicators.ToggleHighlightDown(false);
        }
        SetElevatorReadyToDepart();
    }

    private void SetElevatorReadyToDepart()
    {
        elevator.SetReadyToDepart();
    }
}
