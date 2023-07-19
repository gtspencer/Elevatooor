using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiderV2 : MonoBehaviour
{
    public string riderId;
    public float riderSpeed = 3f;

    public float riderWeight = 150f;

    public BuildingUnit selectedBuildingUnit;
    public ElevatorV2 ridingElevator;
    private int selectedBuildingUnitIndex;
    private float elevatorPosition => ((selectedBuildingUnitIndex) * Building.UNIT_LENGTH);

    private int roomIndex;
    private float roomPosition => (roomIndex * Building.UNIT_LENGTH) + (Building.UNIT_LENGTH / 2);

    public int currentFloor = 1;
    public int destinationFloor = 2;
    
    public Action<RiderV2> OnLeftBuilding = (rider) => { };
    public Action<RiderV2> OnGetOffElevator = (rider) => { };

    private const float STARTING_MOOD_LEVEL = 50;
    private float moodLevel = 50;
    private float waitTime = 0;
    private float rideTime = 0;
    private float stuckTime = 0;
    
    #region Analytics
    private List<int> floorsCalled = new List<int>();

    public Analytics.RiderAnalytics GetRiderAnalytics()
    {
        Analytics.RiderAnalytics analytics = new Analytics.RiderAnalytics()
        {
            riderId = riderId,
            weight = riderWeight,
            moodLevel = moodLevel,
            waitTime = waitTime,
            rideTime = rideTime,
            stuckTime = stuckTime,
            floorsCalled = floorsCalled.ToArray()
        };

        return analytics;
    }
    #endregion

    public enum RiderState
    {
        Idle,
        GoingToElevator,
        WaitingForElevator,
        Riding,
        GoingToDestination,
        StartBusiness,
        Business,
        NoElevatorWander, // state for when no elevator is available, so wander around and check after x seconds
        ImmediateExit
    }

    public RiderState riderState = RiderState.Idle;
    
    public void InitNewRider()
    {
        ResetRider();
        SetRandomRiderProperties();
        GetRandomElevator();
    }

    public void GetRandomElevator()
    {
        // pick random unit, then get ElevatorFloor from unit
        // find random elevator
        var unitsToChoose = Building.Instance.BuildingUnits.Where(unit => unit.Elevator.elevatorState != ElevatorV2.State.OutOfService && unit.Elevator.CanRiderGetOn(riderWeight));

        if (unitsToChoose.Count() <= 0)
        {
            elevatorCheckTimer = 0;
            riderState = RiderState.NoElevatorWander;
            return;
        }

        currentElevatorChecks = 0;
        selectedBuildingUnitIndex = UnityEngine.Random.Range(0, Building.Instance.TotalUnits);
        selectedBuildingUnit = Building.Instance.BuildingUnits[selectedBuildingUnitIndex];

        riderState = RiderState.GoingToElevator;
    }

    private void FixedUpdate()
    {
        switch (riderState)
        {
            case RiderState.GoingToElevator:
                MoveToElevator();
                break;
            case RiderState.WaitingForElevator:
                waitTime += Time.deltaTime;
                CheckMood();
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
            case RiderState.Riding:
                rideTime += Time.deltaTime;
                break;
            case RiderState.NoElevatorWander:
                stuckTime += Time.deltaTime;
                Wander();
                break;
            case RiderState.ImmediateExit:
                JumpOutWindow();
                break;
        }
    }

    private const float ELEVATOR_CHECK_WAIT_TIME = 5f;
    private float elevatorCheckTimer = 0;

    private const int MAX_CHECKS = 10;
    private int currentElevatorChecks = 0;
    private bool riderStuck = false;
    private void Wander()
    {
        // done with checking, just leave already
        if (currentElevatorChecks >= MAX_CHECKS)
        {
            riderStuck = true;
            if (currentFloor == 1)
            {
                riderState = RiderState.GoingToDestination;
            }
            else
            {
                riderState = RiderState.ImmediateExit;
            }
            return;
        }

        elevatorCheckTimer += Time.deltaTime;
        
        // wait time is over, check again
        if (elevatorCheckTimer >= ELEVATOR_CHECK_WAIT_TIME)
        {
            currentElevatorChecks++;
            GetRandomElevator();
        }
        
        // TODO do some wandering
    }

    private void CheckMood()
    {
        
    }

    private void JumpOutWindow()
    {
        
    }

    private float minBusinessTime = 3f;
    private float maxBusinessTime = 10f;
    private IEnumerator DoRandomAmountsOfBusiness()
    {
        riderState = RiderState.Business;
        
        var businessTime = UnityEngine.Random.Range(minBusinessTime, maxBusinessTime);

        yield return new WaitForSeconds(businessTime);

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
            RiderDone();
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
        
        floorsCalled.Add(destinationFloor);
        
        selectedBuildingUnit.floorsToElevatorFloors[currentFloor].RequestRide(this, goingUp);
    }

    public void GetOnElevator(ElevatorV2 elevator)
    {
        riderState = RiderState.Riding;

        this.ridingElevator = elevator;
        ridingElevator.RiderGotOn(this);
        
        this.transform.SetParent(ridingElevator.transform);

        ridingElevator.floorReachedCallbacks[destinationFloor] += DestinationFloorReached;

        ridingElevator.RequestRide(new ElevatorRequest()
        {
            floor = destinationFloor,
            goingUp = destinationFloor > currentFloor,
            riderId = riderId,
            insideRequest = true
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
        
        OnGetOffElevator.Invoke(this);
    }

    private void GetRandomRoom()
    {
        roomIndex = UnityEngine.Random.Range(0, Building.Instance.TotalUnits);
    }
    
    private void RiderDone()
    {
        // ANALYTICS
        Analytics.Instance.AddRider(GetRiderAnalytics());
        
        riderState = RiderState.Idle;
        OnLeftBuilding.Invoke(this);
        
        EventRepository.Instance.OnRiderFinished.Invoke(this);

        GiveGold();
        ResetRider();
    }

    private void GiveGold()
    {
        // TODO calculate multiplier from mood and wait time
        GoldManager.Instance.AddGoldFromCustomer(CalculateGoldMultiplier());
    }

    private float CalculateGoldMultiplier()
    {
        return 1;
    }

    private void SetRandomRiderProperties()
    {
        riderWeight = UnityEngine.Random.Range(120f, 180f);
    }

    private void ResetRider()
    {
        waitTime = 0;
        rideTime = 0;
        stuckTime = 0;
        currentElevatorChecks = 1;
        moodLevel = STARTING_MOOD_LEVEL;
        riderStuck = false;
        riderState = RiderState.Idle;
    }
}
