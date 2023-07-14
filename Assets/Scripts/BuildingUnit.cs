using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnit : MonoBehaviour
{
    [SerializeField] private ElevatorV2 elevator;
    [SerializeField] private GameObject baseElevatorShaft;
    [SerializeField] private GameObject baseRoomShaft;

    public ElevatorV2 Elevator
    {
        get => elevator;
    }

    private int floors = 1;
    private float roomHeight = Building.ROOM_HEIGHT;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeWithFloors(int floors)
    {
        // shafts start with 1 floor
        for (int i = 1; i < floors; i++)
        {
            AddFloor();
        }
    }

    public void AddFloor()
    {
        var shaft = Instantiate(baseElevatorShaft, this.transform);
        shaft.transform.localPosition = new Vector3(0, floors * roomHeight, 0);
        
        var room = Instantiate(baseRoomShaft, this.transform);
        room.transform.localPosition = new Vector3(0, floors * roomHeight, 10);
        
        floors++;
    }
}
