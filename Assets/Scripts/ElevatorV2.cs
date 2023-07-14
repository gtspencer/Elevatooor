using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorV2 : MonoBehaviour
{
    [SerializeField] private bool waitForRiders = false;
    private float defaultElevatorWaitTime = 1f;


    private const float WAIT_TIME_PER_RIDER = 0.5f;
    
    [SerializeField] private Text currentFloorUI;
    [SerializeField] private Text desiredFloorUI;
    [SerializeField] private Text floorQueueUI;
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private int weightLimit = 500; // lbs

    private int currentWeight = 0;

    private List<int> floorQueue = new List<int>();
    
    public int CurrentFloor => (int)(transform.position.y / Building.ROOM_HEIGHT) + 1;
    public int NextFloor => floorQueue.Count > 0 ? floorQueue[0] : -1;

    public Action<int> OnFloorStopped;

    // TEST CODE
    public int callingFloor = 1;
    public bool goingUp = true;
    public bool insertRequest = false;
    
    public enum State
    {
        Up,
        Down,
        Idle,
        PassengersDeparting
    }
    
    public State elevatorState = State.Idle;
    public bool IsElevatorMoving => elevatorState == State.Down || elevatorState == State.Up;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void DoTest()
    {
        insertRequest = false;
        RequestRide(new ElevatorRequest()
        {
            floor = callingFloor,
            goingUp = goingUp
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (insertRequest)
            DoTest();

        SetUI();
        
        if (floorQueue.Count <= 0) return;

        if (elevatorState == State.PassengersDeparting) return;

        MoveElevator(floorQueue[0]);
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

    void MoveElevator(int floor)
    {
        Debug.LogError("Moving to floor " + floor);
        // Calculate the target position for the elevator
        float targetY = (floor - 1) * Building.ROOM_HEIGHT;

        var position = transform.position;

        elevatorState = position.y - targetY > 0 ? State.Down : State.Up;

        // Check if the elevator has reached the target position
        if (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            // Smoothly interpolate the elevator's position towards the target position
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(position.x, targetY, position.z), speed * Time.deltaTime);
        }
        else
        {
            // Set the elevator's position to the exact target position
            transform.position = new Vector3(position.x, targetY, position.z);

            OnFloorStopped?.Invoke(CurrentFloor);
            
            if (waitForRiders)
                StartCoroutine(WaitForPassengersDeparting());
            else
                StartCoroutine(WaitForDefaultTime());
        }
    }

    private IEnumerator WaitForDefaultTime()
    {
        elevatorState = State.PassengersDeparting;

        yield return new WaitForSeconds(defaultElevatorWaitTime);
        
        floorQueue.RemoveAt(0);
        
        ProcessRequestQueue();
        
        elevatorState = State.Idle;
    }

    private IEnumerator WaitForPassengersDeparting()
    {
        elevatorState = State.PassengersDeparting;

        yield return new WaitForSeconds(WAIT_TIME_PER_RIDER);
        
        floorQueue.RemoveAt(0);
        
        ProcessRequestQueue();
        
        elevatorState = State.Idle;
    }

    private List<ElevatorRequest> requestPool = new List<ElevatorRequest>();

    private void ProcessRequestQueue()
    {
        Debug.LogError("Processing request pool");
        
        if (requestPool.Count <= 0)
            return;
        
        if (floorQueue.Count <= 0)
        {
            floorQueue.Add(requestPool[0].floor);

            Debug.LogError("request added " + requestPool[0].floor);
            requestPool.RemoveAt(0);
            
            return;
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
                    
                        Debug.LogError("request added " + r.floor);
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
                    
                        Debug.LogError("request added " + r.floor);
                        AddToFloorQueueDescending(r.floor);
                    }
                }
            }
        }
        
        foreach (ElevatorRequest remove in removeFromPool)
            requestPool.Remove(remove);
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
            floorQueue.Insert(index, value);
        }
        else
        {
            Debug.LogError("Not inserting " + value + " at index " + index);
        }
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
            index = ~index;
            if (index == 0)
            {
                index = 1;
                /*floorQueue.Add(value);
                return;*/
            }

            floorQueue.Insert(index - 1, value);
        }
        else
        {
            Debug.LogError("Not inserting " + value + " at index " + index);
        }
    }

    public void RequestRide(ElevatorRequest request) 
    {
        requestPool.Add(request);
        
        Debug.LogError("Request logged " + request.floor);
        
        ProcessRequestQueue();
        
        /*if (floorQueue.Count <= 0)
        {
            if (CurrentFloor != request.callingFloor)
                floorQueue.Add(request.callingFloor);
            
            floorQueue.Add(request.destinationFloor);

            return;
        }

        bool elevatorMovingUp = NextFloor > CurrentFloor;

        if (elevatorMovingUp)
        {
            if (request.goingUp)
            {
                // calling floor is above current floor
                if (request.callingFloor > CurrentFloor)
                {  
                    // calling floor is between current floor, and destination floor, so stop to pickup (make this next destination)
                    if (NextFloor > request.callingFloor)
                    {
                        floorQueue.Insert(0, callingFloor);

                        for (int i = 1; i < floorQueue.Count; i++)
                        {
                            if (floorQueue[i])
                        }
                    }
                }
            }
            else
            {
                
            }
        }
        else
        {
            if (request.goingUp)
            {
                
            }
            else
            {
                
            }
        }*/
    }
}