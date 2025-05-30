using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Crewmate
{
    public float gearsPerCycle = 7f;

    public Engineer(string crewmateName) : base(crewmateName)
    {
        
    }
    
    public override void PerformDuty()
    {
        base.PerformDuty();

        if (ResourcesManager.Instance.foodSupply >= foodConsumedPerCycle &&
            ResourcesManager.Instance.fuelSupply >= fuelConsumedPerCycle)
        {
            ResourcesManager.Instance.AddGears(gearsPerCycle);
            Debug.Log($"{crewmateName} provided +{gearsPerCycle} gears.");
        }
    }
}
