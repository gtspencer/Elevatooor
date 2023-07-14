using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*[ExecuteInEditMode]*/
public class Building : MonoBehaviour
{
    [SerializeField]
    private const int STARTING_FLOORS = 5;
    [SerializeField]
    private const int STARTING_UNITS = 1;

    public const float UNIT_LENGTH = 20;
    public const float ROOM_HEIGHT = 10;

    [SerializeField] private GameObject UnitPrefab;
    
    private List<BuildingUnit> buildingUnits = new List<BuildingUnit>();
    private List<ElevatorV2> elevators = new List<ElevatorV2>();
    
    private int _floors;

    public int Floors
    {
        get => _floors;
        set
        {
            _floors = value;
        }
    }

    public int Units => buildingUnits.Count;
    
    // Start is called before the first frame update
    void Start()
    {
        Floors = STARTING_FLOORS;
        
        for (int i = 0; i < STARTING_UNITS; i++)
        {
            AddUnit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddUnit()
    {
        var unit = GameObject.Instantiate(UnitPrefab, this.transform);
        unit.transform.position = new Vector3(0, 0, buildingUnits.Count * UNIT_LENGTH);
        var unitScript = unit.GetComponent<BuildingUnit>();

        buildingUnits.Add(unitScript);
        
        unitScript.InitializeWithFloors(Floors);
        
        elevators.Add(unitScript.Elevator);
    }

    public void AddFloor()
    {
        Floors++;
        foreach (BuildingUnit unit in buildingUnits)
        {
            unit.AddFloor();
        }
    }
}
