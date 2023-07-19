using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float startingHour = 7;
    // 5 seconds per 10 minutes
    [SerializeField] private float secondsPerHour = 30;
    private float secondsPer10Minutes;

    // Time is between 0000 and 2400
    private int currentHour;
    private int current10Minute;
    private int currentSingleMinute;
    private int currentDay = 1;

    [SerializeField]
    private Text timeText;
    [SerializeField]
    private Text dayText;
    
    #region Timescale Variables
    [SerializeField] private Text timescaleUI;
    [SerializeField] private Text playPauseUI;
    [SerializeField] private Button slowDownButton;
    [SerializeField] private Button speedUpButton;
    
    private const float MIN_TIMESCALE = 1;
    private const float MAX_TIMESCALE = 16;

    private float currentTimescale = 1;
    #endregion

    #region TimeScale
    private float CurrentTimescale
    {
        get => currentTimescale;
        set
        {
            currentTimescale = value;
            Time.timeScale = value;
            
            UpdateTimeScaleUI();
        }
    }

    private void UpdateTimeScaleUI()
    {
        timescaleUI.text = CurrentTimescale.ToString() + "x";
    }

    public void TogglePlayPause()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            playPauseUI.text = "►";
            slowDownButton.enabled = false;
            speedUpButton.enabled = false;
        }
        else
        {
            Time.timeScale = CurrentTimescale;
            playPauseUI.text = "||";
            slowDownButton.enabled = true;
            speedUpButton.enabled = true;
        }
    }

    public void SpeedUp()
    {
        if (Time.timeScale == 0)
            return;

        if (CurrentTimescale * 2 <= MAX_TIMESCALE)
            CurrentTimescale *= 2;
    }

    public void SlowDown()
    {
        if (Time.timeScale == 0)
            return;

        if (CurrentTimescale / 2 >= MIN_TIMESCALE)
            CurrentTimescale /= 2;
    }
    #endregion
    
    #region Current Time
    // Start is called before the first frame update
    void Start()
    {
        playPauseUI.text = "||";
        
        secondsPer10Minutes = secondsPerHour / 6f;
        currentHour = 7;
        EventRepository.Instance.OnHourChange.Invoke(currentHour);
        SetTimeText();
    }

    private float timer;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        currentSingleMinute = (int)Mathf.Floor(timer * 10 / secondsPer10Minutes);
        if (timer >= secondsPer10Minutes)
        {
            timer = 0;
            currentSingleMinute = 0;
            current10Minute++;
        }

        if (current10Minute >= 6)
        {
            current10Minute = 0;
            currentHour++;
            EventRepository.Instance.OnHourChange.Invoke(currentHour);
        }

        if (currentHour > 24)
        {
            currentHour = 1;
            currentDay++;
        }
        
        SetTimeText();
    }

    public int GetHour()
    {
        return currentHour;
    }

    public int GetMinute()
    {
        return current10Minute * 10;
    }

    private void SetTimeText()
    {
        var isPm = false;
        var displayHour = currentHour;
        if (displayHour >= 12)
            isPm = true;

        if (displayHour > 12)
            displayHour -= 12;

        var displayMinute = currentSingleMinute + (current10Minute * 10);
        
        timeText.text = displayHour.ToString() + ":" + displayMinute.ToString("00") + (isPm ? " pm" : " am");
        dayText.text = "Day " + currentDay.ToString();
    }
    #endregion
}
