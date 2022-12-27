using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float elevatorSpeed = 0.5f;
    [SerializeField] private float elevatorHeight = 3f;
    
    // The time to wait at each floor
    public float waitTime = 2.0f;

    // The time elapsed since the elevator stopped at the current floor
    private float timeElapsed = 0.0f;
    
    [SerializeField]
    private int desiredFloor = 1;

    private List<int> desiredFloors;

    private int minFloor = 1;
    private int maxFloor = 4;

    public Action<int> ElevatorOnRequestedFloor;

    public int CurrentFloor
    {
        get => Mathf.RoundToInt(transform.position.y / floorHeight) + 1;
    }

    public bool IsInUse
    {
        get => desiredFloors.Count > 0;
    }

    public bool IsMovingUp
    {
        get => IsInUse && CurrentFloor < desiredFloors[0];
    }

    private float floorHeight = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        desiredFloors = new List<int>();
        elevatorHeight = this.transform.localScale.y;
        
        AddDesiredFloor(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsInUse)
            return;
        
        // Check if the elevator has reached the destination floor
        if (Mathf.Abs(transform.position.y - (((desiredFloors[0] - 1) * floorHeight)) - (elevatorHeight / 2)) < 0.01f)
        {
            // Update the current floor to match the destination floor
            // currentFloor = desiredFloors[0];

            // Increment the time elapsed
            timeElapsed += Time.deltaTime;

            // Check if the wait time has been reached
            if (timeElapsed >= waitTime)
            {
                // Reset the time elapsed
                timeElapsed = 0.0f;

                ElevatorOnRequestedFloor?.Invoke(desiredFloors[0]);
                
                // Remove the destination floor from the list
                desiredFloors.RemoveAt(0);
            }
        }
        else
        {
            // Move the elevator towards the next destination floor
            MoveElevator(desiredFloors[0]);
        }
        
        /*// Check if the desired floor has changed
        if (desiredFloor != previousFloor)
        {
            // Move the elevator to the desired floor
            MoveElevator(desiredFloor);

            // Update the current floor to match the desired floor
            // previousFloor = desiredFloor;
            // LeanTween.moveY(this.gameObject, (desiredFloor - 1) * floorHeight, desiredFloor * 3f);
        }*/
        // Debug.LogError("Current Floor: " + CurrentFloor + "\nDesired Floor: " + desiredFloor + "\nPrevious Floor: " + previousFloor);
    }
    
    void MoveElevator(int floor)
    {
        // Calculate the target position for the elevator
        float targetY = (floor - 1) * floorHeight;
        
        // account for size of elevator
        targetY += elevatorHeight / 2;

        // Smoothly interpolate the elevator's position towards the target position
        // transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), elevatorSpeed * Time.deltaTime);

        // Check if the elevator has reached the target position
        if (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            // Smoothly interpolate the elevator's position towards the target position
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), elevatorSpeed * Time.deltaTime);
        }
        else
        {
            // TODO this never gets hit since we check for this same thing in the update method
            
            // Set the elevator's position to the exact target position
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }
    }

    public void AddDesiredFloor(int floor)
    {
        floor = Mathf.Clamp(floor, minFloor, maxFloor);

        if (CurrentFloor > floor && IsMovingUp)
        {
            desiredFloors.Add(floor);
            return;
        } else if (CurrentFloor < floor && !IsMovingUp)
        {
            desiredFloors.Add(floor);
            return;
        }

        int index = 0;
        while (index < desiredFloors.Count && desiredFloors[index] < floor)
        {
            // don't add already added floors
            if (desiredFloors[index] == floor)
                return;
            
            index++;
        }
        desiredFloors.Insert(index, floor);
    }

    // Maybe this should be done in the dispatcher?  elevators should only know which list of floors they are going to next
    // dispatcher just populates the list
    private void OrganizeDesiredFloors()
    {
        List<int> greaterFloors = new List<int>();
        List<int> lesserFloors = new List<int>();

        foreach (int floor in desiredFloors)
        {
            if (floor > CurrentFloor)
            {
                int index = 0;
                while (index < greaterFloors.Count && greaterFloors[index] < floor)
                {
                    index++;
                }
                greaterFloors.Insert(index, floor);
            } else if (floor < CurrentFloor)
            {
                int index = 0;
                while (index < lesserFloors.Count && lesserFloors[index] < floor)
                {
                    index++;
                }
                lesserFloors.Insert(index, floor);
            }
        }
    }
    
    void OnGUI()
    {
        string floors = "";
        foreach (int i in desiredFloors)
        {
            floors += i.ToString() + ", ";
        }

        if (floors.Length > 0)
            floors.Substring(0, floors.Length - 2);
        
        GUI.TextArea(new Rect(new Vector2(0, 0), new Vector2(150, 20)), "Current Floor: " + CurrentFloor);
        GUI.TextArea(new Rect(new Vector2(0, 20), new Vector2(150, 20)), "Desired Floors " + floors);
    }
}
