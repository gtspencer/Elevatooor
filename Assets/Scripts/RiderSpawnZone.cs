using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiderSpawnZone : MonoBehaviour
{
    [SerializeField] private GameObject riderPrefab;

    private List<GameObject> ridersPool = new List<GameObject>();

    [SerializeField] private int numPooledRiders = 10;
    private int currentIndex = 0;
    
    public static RiderSpawnZone Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ridersPool = new List<GameObject>();

        for (int i = 0; i < numPooledRiders; i++)
        {
            CreateNewRider();
        }
    }
    
    private void CreateNewRider()
    {
        GameObject obj = Instantiate(riderPrefab, transform);
        obj.SetActive(false);
        ridersPool.Add(obj);
    }

    public GameObject GetObjectFromPool()
    {
        for (int i = 0; i < ridersPool.Count; i++)
        {
            currentIndex = (currentIndex + 1) % ridersPool.Count;
            if (!ridersPool[currentIndex].activeInHierarchy)
            {
                ridersPool[currentIndex].SetActive(true);
                return ridersPool[currentIndex];
            }
        }

        CreateNewRider();
        currentIndex = ridersPool.Count - 1;
        ridersPool[currentIndex].SetActive(true);
        return ridersPool[currentIndex];
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.transform.SetParent(this.transform);
        obj.SetActive(false);
    }
}
