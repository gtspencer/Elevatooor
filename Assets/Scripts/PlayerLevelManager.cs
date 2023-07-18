using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Leveling up unlocks new things to build, but also increases riders coming
/// </summary>
public class PlayerLevelManager : MonoBehaviour
{
    [SerializeField] private const int STARTING_LEVEL = 1;

    [SerializeField] private int currentLevel;
    // Start is called before the first frame update
    void Start()
    {
        currentLevel = STARTING_LEVEL;
    }

    public void IncreaseLevel()
    {
        currentLevel++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
