using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiderV2 : MonoBehaviour
{
    public int riderId;
    public float riderSpeed = 3f;

    public ElevatorV2 ridingElevator;
    private int elevatorIndex;
    private float elevatorPosition => ((elevatorIndex) * Building.UNIT_LENGTH);

    private int roomIndex;
    private float roomPosition => (roomIndex * Building.UNIT_LENGTH) + (Building.UNIT_LENGTH / 2);

    private int currentFloor = 1;
    private int destinationFloor = 2;

    public Action<RiderV2> OnLeftBuilding;
    
    public enum RiderState
    {
        Idle,
        GoingToElevator,
        WaitingForElevator,
        Riding,
        GoingToDestination,
        StartBusiness,
        Business
    }

    public RiderState riderState = RiderState.Idle;

    public void GetRandomElevator()
    {
        // find random elevator
        elevatorIndex = UnityEngine.Random.Range(0, Building.Instance.elevators.Count);
        ridingElevator = Building.Instance.elevators[elevatorIndex];

        riderState = RiderState.GoingToElevator;
    }

    private void Update()
    {
        switch (riderState)
        {
            case RiderState.GoingToElevator:
                MoveToElevator();
                break;
            case RiderState.WaitingForElevator:
                // do nothing, wait for callback
                break;
            case RiderState.GoingToDestination:
                if (currentFloor == 1)
                    MoveToSpawnZone();
                else
                    MoveToRoom();
                break;
            case RiderState.StartBusiness:
                StartCoroutine(DoRandomAmountsOfBusiness());
                break;
            case RiderState.Business:
                break;
        }
    }

    private float minBusinessTime = 3f;
    private float maxBusinessTime = 10f;
    private IEnumerator DoRandomAmountsOfBusiness()
    {
        riderState = RiderState.Business;
        
        var waitTime = UnityEngine.Random.Range(minBusinessTime, maxBusinessTime);

        yield return new WaitForSeconds(waitTime);

        GetRandomElevator();
    }

    private void MoveToElevator()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, elevatorPosition), riderSpeed * Time.deltaTime);
        
        if (Mathf.Abs(transform.position.z - elevatorPosition) < 0.01f)
        {
            // Set the elevator's position to the exact target position
            transform.position = new Vector3(transform.position.x, transform.position.y, elevatorPosition);
            RequestRide();
        }
    }
    
    private void MoveToSpawnZone()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, RiderSpawnZone.Instance.transform.position.z), riderSpeed * Time.deltaTime);
        
        if (Mathf.Abs(transform.position.z - RiderSpawnZone.Instance.transform.position.z) < 0.01f)
        {
            // Set the elevator's position to the exact target position
            transform.position = new Vector3(transform.position.x, transform.position.y, RiderSpawnZone.Instance.transform.position.z);
            LeaveBuilding();
        }
    }

    private void MoveToRoom()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, roomPosition), riderSpeed * Time.deltaTime);
        
        if (Mathf.Abs(transform.position.z - roomPosition) < 0.01f)
        {
            // Set the elevator's position to the exact target position
            transform.position = new Vector3(transform.position.x, transform.position.y, roomPosition);
            riderState = RiderState.StartBusiness;
        }
    }

    private void RequestRide()
    {
        riderState = RiderState.WaitingForElevator;
        
        ElevatorRequest request = new ElevatorRequest()
        {
            floor = currentFloor,
            riderId = riderId
        };
        
        // if on floor 1, definitely going up
        if (currentFloor == 1)
        {
            destinationFloor = UnityEngine.Random.Range(2, Building.Instance.Floors);

            request.goingUp = true;
        }
        else
        {
            // TODO randomly select another floor for multi floor customers?
            destinationFloor = 1;

            request.goingUp = false;
        }
        
        ridingElevator.OnFloorStopped += FloorReached;
        
        if (ridingElevator.CurrentFloor == currentFloor && !ridingElevator.IsElevatorMoving)
            FloorReached(currentFloor);
        else
            ridingElevator.RequestRide(request);
    }

    private void FloorReached(int floor)
    {
        if (riderState != RiderState.Riding)
        {
            if (currentFloor == floor)
            {
                GetOnElevator();
            }
        }
        else
        {
            if (destinationFloor == floor)
            {
                currentFloor = floor;
                
                GetOffElevator();
            }
        }
    }

    public void GetOnElevator()
    {
        riderState = RiderState.Riding;
        
        this.transform.SetParent(ridingElevator.transform);

        ridingElevator.RequestRide(new ElevatorRequest()
        {
            floor = destinationFloor,
            goingUp = destinationFloor > currentFloor,
            riderId = riderId
        });
    }

    public void GetOffElevator()
    {
        this.transform.SetParent(null);
        
        ridingElevator.OnFloorStopped -= FloorReached;
        ridingElevator = null;

        if (currentFloor != 1)
            GetRandomRoom();
        
        riderState = RiderState.GoingToDestination;
    }

    private void GetRandomRoom()
    {
        roomIndex = UnityEngine.Random.Range(0, Building.Instance.Units);
    }
    
    private void LeaveBuilding()
    {
        riderState = RiderState.Idle;
        OnLeftBuilding?.Invoke(this);
    }
}
