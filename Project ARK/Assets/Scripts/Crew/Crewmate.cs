using System;
using System.Collections.Generic;
using UnityEngine;

public class Crewmate : MonoBehaviour
{
    public string crewmateName;
    public float health;

    public float foodConsumedPerCycle = 1f;
    public float fuelConsumedPerCycle = 0.5f;
    public float resourcesConsumed;

    //Constructor
    public Crewmate(string crewmateName)
    {
        this.crewmateName = crewmateName;
    }
    
    public static List<Crewmate> AllCrewmates;

    public static Crewmate FindCrewmate(string name)
    {
        if (AllCrewmates == null || AllCrewmates.Count == 0)
            return null;
            
        foreach (Crewmate crewmate in AllCrewmates)
        {
            if (crewmate.crewmateName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return crewmate;
            }
        }
        
        return null;
    }
    
    private void Awake()
    {
        if (AllCrewmates == null)
        {
            AllCrewmates = new List<Crewmate>();
        }
        AllCrewmates.Add(this);
    }

    // Virtual method that derived classes can override
    public virtual void PerformDuty()
    {
        bool hasFood = ResourcesManager.Instance.ConsumeFood(foodConsumedPerCycle);
        bool hasFuel = ResourcesManager.Instance.ConsumeFuel(fuelConsumedPerCycle);

        if (!hasFood || !hasFuel)
        {
            Debug.Log($"{crewmateName} cannot perform duty due to lack of food or fuel.");
            return;
        }

        Debug.Log($"{crewmateName} is performing their duty.");
    }
}