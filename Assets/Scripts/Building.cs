using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*[ExecuteInEditMode]*/
public class Building : MonoBehaviour
{
    public static Building Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    [SerializeField]
    private const int STARTING_FLOORS = 5;
    [SerializeField]
    private const int STARTING_UNITS = 1;

    public const float UNIT_LENGTH = 20;
    public const float ROOM_HEIGHT = 10;

    [SerializeField] private GameObject UnitPrefab;
    [SerializeField] private GameObject RightWallPrefab;

    private List<BuildingUnit> buildingUnits = new List<BuildingUnit>();

    private Dictionary<int, GameObject> floorsToRightWalls = new Dictionary<int, GameObject>();
    // public List<ElevatorV2> elevators = new List<ElevatorV2>();

    private int _floors;

    public int Floors
    {
        get => _floors;
        set
        {
            _floors = value;
        }
    }

    public int TotalUnits => buildingUnits.Count;
    public List<BuildingUnit> BuildingUnits => buildingUnits;
    
    // Start is called before the first frame update
    void Start()
    {
        Floors = STARTING_FLOORS;
        
        for (int i = 1; i <= STARTING_UNITS; i++)
        {
            AddUnit();
        }
    }

    public void AddUnit()
    {
        var unit = GameObject.Instantiate(UnitPrefab, this.transform);
        
        unit.transform.position = new Vector3(0, 0, buildingUnits.Count * UNIT_LENGTH);
        var unitScript = unit.GetComponent<BuildingUnit>();

        buildingUnits.Add(unitScript);
        unit.name = "Unit " + TotalUnits;

        unitScript.unitNumber = TotalUnits;
        
        unitScript.InitializeWithFloors(Floors);

        // move right wall over
        for (int i = 1; i <= Floors; i++)
        {
            if (!floorsToRightWalls.ContainsKey(i))
                floorsToRightWalls.Add(i, Instantiate(RightWallPrefab, this.transform));

            floorsToRightWalls[i].transform.localPosition = new Vector3(0, i * ROOM_HEIGHT - (ROOM_HEIGHT / 2), TotalUnits * UNIT_LENGTH - (ROOM_HEIGHT / 2));
        }
    }

    public void AddFloor()
    {
        Floors++;
        foreach (BuildingUnit unit in buildingUnits)
        {
            unit.AddFloor();
        }
        
        if (!floorsToRightWalls.ContainsKey(Floors))
            floorsToRightWalls.Add(Floors, Instantiate(RightWallPrefab, this.transform));
        
        floorsToRightWalls[Floors].transform.localPosition = new Vector3(0, Floors * ROOM_HEIGHT - (ROOM_HEIGHT / 2), TotalUnits * UNIT_LENGTH - (ROOM_HEIGHT / 2));
    }
}
