using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Analytics : MonoBehaviour
{
    public static Analytics Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    private string riderFileName = "RiderAnalysis.json";
    private string elevatorFileName = "ElevatorAnalysis.json";

    private string riderFilePath => Path.Combine(folderPath, riderFileName);
    private string elevatorFilePath => Path.Combine(folderPath, elevatorFileName);

    private string folderPath;

    private ElevatorAnalytics elevatorAnalysis;
    private List<int> floorsTraveled;
    private List<int> requestsAddedToPool;
    
    private List<RiderAnalytics> allRiders;
    private RiderListAnalytics riderAnalysis;

    private void Start()
    {
        string folderName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        
        // Set the file path for the JSON file
        folderPath = Application.dataPath + $"/AnalysisFiles/{folderName}";
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Create an empty list of riders
        allRiders = new List<RiderAnalytics>();
    }

    private void SaveToJson()
    {
        // Convert the list of riders to JSON format
        string riderJson = JsonUtility.ToJson(riderAnalysis, true);
        string elevatorJson = JsonUtility.ToJson(elevatorAnalysis, true);

        // Write the JSON data to the file
        File.WriteAllText(riderFilePath, riderJson);
        File.WriteAllText(elevatorFilePath, elevatorJson);
    }

    public void AddRider(RiderAnalytics riderAnalytics)
    {
        // Add the rider to the list
        allRiders.Add(riderAnalytics);
    }

    private void OnApplicationQuit()
    {
        riderAnalysis = new RiderListAnalytics()
        {
            riders = allRiders,
            totalRiders = allRiders.Count
        };

        SetRiderData();
        SetElevatorData();
        
        SaveToJson();
    }

    // TODO do for each elevator
    private void SetElevatorData()
    {
        elevatorAnalysis = new ElevatorAnalytics()
        {
            floorsTraveled = floorsTraveled,
            requestsAddedToPool = requestsAddedToPool
        };
    }

    private void SetRiderData()
    {
        float totalWaitTime = 0;
        float totalRideTime = 0;
        float totalStuckTime = 0;
        
        foreach (RiderAnalytics riderAnal in allRiders)
        {
            totalWaitTime += riderAnal.waitTime;
            totalRideTime += riderAnal.rideTime;
            totalStuckTime += riderAnal.stuckTime;
        }

        riderAnalysis = new RiderListAnalytics()
        {
            totalRiders = allRiders.Count,
            riders = allRiders
        };

        riderAnalysis.averageWaitTime = totalWaitTime / riderAnalysis.totalRiders;
        riderAnalysis.averageRideTime = totalRideTime / riderAnalysis.totalRiders;
        riderAnalysis.averageStuckTime = totalStuckTime / riderAnalysis.totalRiders;
    }

    [System.Serializable]
    public class RiderAnalytics
    {
        public string riderId;
        public float weight;
        public float moodLevel;
        public float waitTime;
        public float rideTime;
        public float stuckTime;
        public int[] floorsCalled;
    }

    [System.Serializable]
    public class RiderListAnalytics
    {
        public int totalRiders;
        public float averageWaitTime;
        public float averageRideTime;
        public float averageStuckTime;
        public List<RiderAnalytics> riders;
    }
    
    [System.Serializable]
    public class ElevatorAnalytics
    {
        public List<int> floorsTraveled;
        public List<int> requestsAddedToPool;
    }
}
