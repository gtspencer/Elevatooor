using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUnit : MonoBehaviour
{
    [SerializeField] private ElevatorV2 elevator;
    [SerializeField] private GameObject baseElevatorShaft;
    [SerializeField] private GameObject baseRoomShaft;
    
    [SerializeField] private GameObject LeftWallPrefab;

    [SerializeField] private GameObject RoomRoofPrefab;
    [SerializeField] private GameObject ShaftRoofPrefab;

    private GameObject currentShaftRoof;
    private GameObject currentRoomRoof;

    public int unitNumber;

    // holds reference to each floor of the elevator
    public Dictionary<int, ElevatorFloor> floorsToElevatorFloors = new Dictionary<int, ElevatorFloor>();
    
    public ElevatorV2 Elevator
    {
        get => elevator;
    }

    private int floors = 0;
    private float roomHeight = Building.ROOM_HEIGHT;

    public void InitializeWithFloors(int floors)
    {
        /*floorsToElevatorFloors.Add(1, baseElevatorShaft.GetComponentInChildren<ElevatorFloor>());
        baseElevatorShaft.name = "Shaft 1";*/
        // shafts start with 1 floor
        for (int i = 0; i < floors; i++)
        {
            AddFloor();
        }
    }

    public void AddFloor()
    {
        floors++;
        
        var shaft = Instantiate(baseElevatorShaft, this.transform);
        shaft.transform.localPosition = new Vector3(0, (floors - 1) * roomHeight, 0);
        shaft.name = "Shaft " + floors;
        shaft.SetActive(true);

        var elevatorFloor = shaft.GetComponentInChildren<ElevatorFloor>();
        elevatorFloor.InitFloor(floors, elevator);

        floorsToElevatorFloors.Add(floors, elevatorFloor);
        
        var room = Instantiate(baseRoomShaft, this.transform);
        room.transform.localPosition = new Vector3(0, (floors - 1) * roomHeight, 10);
        room.name = "Room " + floors;
        room.SetActive(true);

        // add left wall
        if (unitNumber == 1)
        {
            var leftWallPiece = Instantiate(LeftWallPrefab, this.transform);
            leftWallPiece.transform.position = new Vector3(0, (floors * Building.ROOM_HEIGHT) - 5, -5);
        }

        // handle roofs
        if (floors == Building.Instance.Floors)
        {
            if (currentRoomRoof == null)
                currentRoomRoof = Instantiate(RoomRoofPrefab, this.transform);

            if (currentShaftRoof == null)
                currentShaftRoof = Instantiate(ShaftRoofPrefab, this.transform);

            currentRoomRoof.transform.localPosition = new Vector3(0, floors * Building.ROOM_HEIGHT, 10);
            currentShaftRoof.transform.localPosition = new Vector3(0, floors * Building.ROOM_HEIGHT, 0);
        }
    }
}
