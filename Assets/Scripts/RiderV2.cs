using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiderV2 : MonoBehaviour
{
    public string riderId;
    public float riderSpeed = 3f;

    public BuildingUnit selectedBuildingUnit;
    public ElevatorV2 ridingElevator;
    private int selectedBuildingUnitIndex;
    private float elevatorPosition => ((selectedBuildingUnitIndex) * Building.UNIT_LENGTH);

    private int roomIndex;
    private float roomPosition => (roomIndex * Building.UNIT_LENGTH) + (Building.UNIT_LENGTH / 2);

    public int currentFloor = 1;
    public int destinationFloor = 2;

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
        // pick random unit, then get ElevatorFloor from unit
        // find random elevator
        selectedBuildingUnitIndex = UnityEngine.Random.Range(0, Building.Instance.TotalUnits);
        selectedBuildingUnit = Building.Instance.BuildingUnits[selectedBuildingUnitIndex];

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
            RequestInitialRide();
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

    private void RequestInitialRide()
    {
        riderState = RiderState.WaitingForElevator;

        bool goingUp = false;
        // if on floor 1, definitely going up
        if (currentFloor == 1)
        {
            destinationFloor = UnityEngine.Random.Range(2, Building.Instance.Floors);

            goingUp = true;
        }
        else
        {
            // TODO randomly select another floor for multi floor customers?
            destinationFloor = 1;
        }
        
        selectedBuildingUnit.floorsToElevatorFloors[currentFloor].RequestRide(this, goingUp);
    }

    public void GetOnElevator(ElevatorV2 elevator)
    {
        riderState = RiderState.Riding;

        this.ridingElevator = elevator;
        
        this.transform.SetParent(ridingElevator.transform);

        ridingElevator.floorReachedCallbacks[destinationFloor] += DestinationFloorReached;

        ridingElevator.RequestRide(new ElevatorRequest()
        {
            floor = destinationFloor,
            goingUp = destinationFloor > currentFloor,
            riderId = riderId
        });
    }

    private void DestinationFloorReached()
    {
        ridingElevator.floorReachedCallbacks[destinationFloor] -= DestinationFloorReached;

        currentFloor = destinationFloor;
        
        GetOffElevator();
    }

    public void GetOffElevator()
    {
        this.transform.SetParent(null);
        
        ridingElevator = null;

        if (currentFloor != 1)
            GetRandomRoom();
        
        riderState = RiderState.GoingToDestination;
    }

    private void GetRandomRoom()
    {
        roomIndex = UnityEngine.Random.Range(0, Building.Instance.TotalUnits);
    }
    
    private void LeaveBuilding()
    {
        riderState = RiderState.Idle;
        OnLeftBuilding?.Invoke(this);
    }
}
