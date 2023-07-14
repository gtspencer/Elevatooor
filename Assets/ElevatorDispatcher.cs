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
        if (elevatorRequests.Count <= 0)
            return;
        
        var request = elevatorRequests.Dequeue();
        var elevator = GetBestElevator(request.callingFloor, request.destinationFloor);

        if (elevator == null)
        {
            elevatorRequests.Enqueue(request);
            return;
        }
        
        elevator.AddDesiredFloor(request.callingFloor);
    }

    public void RequestElevator(Rider r, int callingFloor, int destinationFloor)
    {
        // .Enqueue(new ElevatorRequest { rider = r, callingFloor = callingFloor, destinationFloor = destinationFloor });
    }

    public Elevator GetBestElevator(int callingFloor, int destinationFloor)
    {
        bool movingUp = callingFloor < destinationFloor;

        Elevator bestElevator = null;
        int bestElevatorScore = 0;

        foreach (Elevator e in elevators)
        {
            int currentElevatorScore = 0;
            if (!e.IsInUse)
                currentElevatorScore++;

            if (e.elevatorState == Elevator.State.Up && movingUp && callingFloor > e.CurrentFloor)
                currentElevatorScore++;
            else if (e.elevatorState == Elevator.State.Down && !movingUp && callingFloor < e.CurrentFloor)
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
