using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : Crewmate
{
    public float healthPerCycle = 12f;

    public Medic(string crewmateName) : base(crewmateName)
    {
        
    }
    
    public override void PerformDuty()
    {
        base.PerformDuty();

        if (ResourcesManager.Instance.foodSupply >= foodConsumedPerCycle &&
            ResourcesManager.Instance.fuelSupply >= fuelConsumedPerCycle)
        {
            ResourcesManager.Instance.AddHealth(healthPerCycle);
            Debug.Log($"{crewmateName} provided +{healthPerCycle} health.");
        }
    }
}
