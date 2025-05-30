using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enforcer : Crewmate
{
    public float securityPerCycle = 8f;

    public Enforcer(string crewmateName) : base(crewmateName)
    {
        
    }
    
    public override void PerformDuty()
    {
        base.PerformDuty();

        if (ResourcesManager.Instance.foodSupply >= foodConsumedPerCycle &&
            ResourcesManager.Instance.fuelSupply >= fuelConsumedPerCycle)
        {
            ResourcesManager.Instance.AddSecurity(securityPerCycle);
            Debug.Log($"{crewmateName} provided +{securityPerCycle} security.");
        }
    }
}
