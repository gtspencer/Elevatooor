using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorV2 : MonoBehaviour
{
    [SerializeField] private BuildingUnit buildingUnit;
    
    [SerializeField] private bool waitForRiders = true;
    private float defaultElevatorWaitTime = 1f;
    private const float WAIT_TIME_PER_RIDER = 0.5f;
    
    [SerializeField] private Text currentFloorUI;
    [SerializeField] private Text desiredFloorUI;
    [SerializeField] private Text floorQueueUI;

    private float currentSpeed;
    private float currentLerpTime;
    private float lerpDuration = 1f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private int weightLimit = 500; // lbs

    [SerializeField] private bool showAllFloorsInQueue = true;
    public Action OnLitButtonsChanged;
    private List<int> litButtons = new List<int>();
    public List<int> LitButtons => litButtons;

    private int currentWeight = 0;

    [SerializeField]
    private List<int> floorQueue = new List<int>();
    
    public int CurrentFloor => (int)(transform.position.y / Building.ROOM_HEIGHT) + 1;
    public int NextFloor => floorQueue.Count > 0 ? floorQueue[0] : -1;
    
    public Dictionary<int, Action> floorReachedCallbacks = new Dictionary<int, Action>();

    public enum State
    {
        Idle,
        Up,
        Down,
        DoorsOpening,
        PassengersExchanging,
        DoorsClosing,
    }
    
    public State elevatorState = State.Idle;
    public bool IsElevatorMoving => elevatorState == State.Down || elevatorState == State.Up;

    public bool IsDoorMoving => !IsElevatorMoving && (elevatorState == State.DoorsClosing || elevatorState == State.DoorsOpening);
    
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Elevator");
    }

    public void AddFloorReachedCallback(int floor, Action callback)
    {
        floorReachedCallbacks.Add(floor, () => { });
        floorReachedCallbacks[floor] += callback;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetUI();
        
        if (floorQueue.Count <= 0) return;

        if (elevatorState == State.PassengersExchanging) return;
        
        if (IsDoorMoving) return;

        MoveElevatorToFloor(floorQueue[0]);
    }

    private void SetUI()
    {
        currentFloorUI.text = CurrentFloor.ToString();

        desiredFloorUI.text = NextFloor != -1 ? NextFloor.ToString() : "";

        floorQueueUI.text = GetDesiredFloorString();
    }
    
    private string GetDesiredFloorString()
    {
        string floors = "";
        foreach (int i in floorQueue)
        {
            floors += i.ToString() + ", ";
        }

        if (floors.Length > 0)
            floors.Substring(0, floors.Length - 2);

        return floors;
    }

    void MoveElevatorToFloor(int floor)
    {
        // Calculate the target position for the elevator
        float targetY = (floor - 1) * Building.ROOM_HEIGHT;

        var position = transform.position;

        elevatorState = position.y - targetY > 0 ? State.Down : State.Up;

        // Check if the elevator has reached the target position
        if (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            // Smoothly interpolate the elevator's position towards the target position
            float t = maxSpeed * Time.deltaTime;
            float normalizedT = Mathf.SmoothStep(0f, 1f, t);
            
            transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, targetY, position.z), normalizedT);


            /*if (currentLerpTime < lerpDuration)
            {
                currentLerpTime += Time.deltaTime;
                float t = currentLerpTime / lerpDuration;

                if (t < 0.5f)
                    currentSpeed = Mathf.Lerp(0f, maxSpeed, 2f * t);
                else
                {
                    float decelerationT = (t - .5f) * 2f;
                    currentSpeed = Mathf.Lerp(maxSpeed, 0f, decelerationT);
                }
                
                Vector3 newPosition = transform.position + transform.forward * currentSpeed * Time.deltaTime;
                transform.position = newPosition;
            }
            else
            {
                transform.position = new Vector3(position.x, targetY, position.z);
            }*/
        }
        else
        {
            // Set the elevator's position to the exact target position
            transform.position = new Vector3(position.x, targetY, position.z);

            // TODO either default time so people get chopped in half, or everyone gets on (sensor upgrade)
            StartCoroutine(OpenDoors());
        }
    }

    private float doorsOpenTime = 1f;
    private void NotifyFloorReady()
    {
        // TODO use CurrentFloor or floorQueue[0]??
        if (litButtons.Contains(CurrentFloor))
        {
            litButtons.Remove(CurrentFloor);
            OnLitButtonsChanged?.Invoke();
        }
        
        floorQueue.RemoveAt(0);
        floorReachedCallbacks[CurrentFloor]?.Invoke();
    }

    public void SetReadyToDepart()
    {
        StartCoroutine(CloseDoors());
    }

    private IEnumerator OpenDoors()
    {
        elevatorState = State.DoorsOpening;
        
        // TODO animation
        yield return new WaitForSeconds(doorsOpenTime);
        
        elevatorState = State.PassengersExchanging;
        
        NotifyFloorReady();
    }
    
    private IEnumerator CloseDoors()
    {
        elevatorState = State.DoorsClosing;
        
        // TODO animation
        yield return new WaitForSeconds(doorsOpenTime);

        elevatorState = State.Idle;
        
        ProcessRequestQueue();
    }

    [SerializeField]
    private List<ElevatorRequest> requestPool = new List<ElevatorRequest>();

    private bool handleRequestsByTime = true;

    public void ProcessRequestQueue()
    {
        /*if (handleRequestsByTime)
        {
            HandleRequestsByTime();
            return;
        }*/
            
        // no requests
        if (requestPool.Count <= 0)
            return;
        
        // nothing in queue, so add first request
        if (floorQueue.Count <= 0)
        {
            floorQueue.Add(requestPool[0].floor);

            requestPool.RemoveAt(0);
        }

        bool elevatorMovingUp = NextFloor > CurrentFloor;
        List<ElevatorRequest> removeFromPool = new List<ElevatorRequest>();
        if (elevatorMovingUp)
        {
            foreach (ElevatorRequest r in requestPool)
            {
                // TODO Catch case where both elevator and rider are moving up, but elevator is above the calling floor
                if (r.goingUp)
                {
                    if (CurrentFloor < r.floor)
                    {
                        removeFromPool.Add(r);
                    
                        AddToFloorQueueAscending(r.floor);
                    }
                }
            }
        }
        else
        {
            foreach (ElevatorRequest r in requestPool)
            {
                // TODO Catch case where both elevator and rider are moving down, but elevator is below the calling floor
                if (!r.goingUp)
                {
                    if (CurrentFloor > r.floor)
                    {
                        removeFromPool.Add(r);
                    
                        AddToFloorQueueDescending(r.floor);
                    }
                }
            }
        }
        
        foreach (ElevatorRequest remove in removeFromPool)
            requestPool.Remove(remove);
    }

    private void HandleRequestsByTime()
    {
        foreach (ElevatorRequest request in requestPool)
        {
            if (!floorQueue.Contains(request.floor))
                floorQueue.Add(request.floor);
        }
        
        requestPool.Clear();
    }

    private void AddToFloorQueueAscending(int value)
    {
        if (floorQueue.Contains(value))
            return;
        
        if (floorQueue.Count <= 0)
        {
            floorQueue.Add(value);
            return;
        }
        
        int index = floorQueue.BinarySearch(value);
        if (index < 0)
        {
            index = ~index;
        }
        
        floorQueue.Insert(index, value);
    }
    
    private void AddToFloorQueueDescending(int value)
    {
        if (floorQueue.Contains(value))
            return;

        if (floorQueue.Count <= 0)
        {
            floorQueue.Add(value);
            return;
        }

        int index = floorQueue.BinarySearch(value);
        if (index < 0)
        {
            index = ~index - 1;
            
            if (index < 0)
            {
                index = 0;
            }
        }

        floorQueue.Insert(index, value);
    }

    public void RequestRide(ElevatorRequest request)
    {
        requestPool.Add(request);

        if (!litButtons.Contains(request.floor) && (request.insideRequest || showAllFloorsInQueue))
        {
            litButtons.Add(request.floor);
            OnLitButtonsChanged?.Invoke();
        }
        
        ProcessRequestQueue();
    }
}