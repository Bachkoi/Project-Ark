using System;
using System.Collections.Generic;
using UnityEngine;

public class Crewmate : MonoBehaviour
{
    public string crewmateName;
    public float health;
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
        Debug.Log($"{crewmateName} is performing their duty.");
    }
}