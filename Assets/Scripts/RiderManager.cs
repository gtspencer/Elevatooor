using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiderManager : MonoBehaviour
{
    [SerializeField] private RiderSpawnZone riderSpawnZone;
    
    public void SendRider()
    {
        var newRider = riderSpawnZone.GetObjectFromPool();

        var riderScript = newRider.GetComponent<RiderV2>();

        riderScript.OnLeftBuilding += RiderLeftBuilding;
        
        riderScript.GetRandomElevator();
    }

    private void RiderLeftBuilding(RiderV2 rider)
    {
        riderSpawnZone.ReturnObjectToPool(rider.gameObject);
    }
}
