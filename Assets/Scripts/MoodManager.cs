using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages things influencing a riders mood
/// </summary>
public class MoodManager : MonoBehaviour
{
    public static MoodManager Instance { get; private set; }

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
