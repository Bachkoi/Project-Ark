using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    // Establish Necessary fields
    [BoxGroup("Time")] public int timeCycle;
    [BoxGroup("Time")] public int daysPerCycle;
    [BoxGroup("Time")] public int day;
    [BoxGroup("Time")] public float hour;

    [BoxGroup("External Controls")] public bool timePaused;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {



            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timePaused == false)
        {
            hour += Time.deltaTime/4.0f;
            if (hour > 24)
            {
                AdvanceDay();
            }
            
        }
    }

    public int DaysUntilNextCycle()
    {
        return daysPerCycle - day;
    }

    public void AdvanceDay()
    {
        Debug.Log("Day Advanced");
        day++;
        hour = 0.0f;
        if (day >= daysPerCycle)
        {
            AdvanceTimeCycle();    
        }
    }

    public void AdvanceTimeCycle()
    {
        Debug.Log("Time Cycle Advanced");
        timeCycle++;
        day = 0;
        hour = 0.0f;
        // Do other time cycle related actions
    }
}
