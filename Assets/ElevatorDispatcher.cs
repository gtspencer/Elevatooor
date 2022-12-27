using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDispatcher : MonoBehaviour
{
    [SerializeField]
    private List<Elevator> elevators;

    private Queue<ElevatorRequest> elevatorRequests;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestElevator(int callingFloor, int destinationFloor)
    {
        elevatorRequests.Enqueue(new ElevatorRequest { callingFloor = callingFloor, destinationFloor = destinationFloor });
    }

    public Elevator GetBestElevator(int callingFloor, int destinationFloor)
    {
        bool movingUp = callingFloor < destinationFloor;

        Elevator bestElevator = elevators[0];
        int bestElevatorScore = 0;

        foreach (Elevator e in elevators)
        {
            int currentElevatorScore = 0;
            if (!e.IsInUse)
                currentElevatorScore++;

            if (e.IsMovingUp && movingUp && callingFloor > e.CurrentFloor)
                currentElevatorScore++;
            else if (!e.IsMovingUp && !movingUp && callingFloor < e.CurrentFloor)
                currentElevatorScore++;

            if (currentElevatorScore > bestElevatorScore)
            {
                bestElevator = e;
                bestElevatorScore = currentElevatorScore;
            }
        }
        
        return bestElevator;
    }
}
