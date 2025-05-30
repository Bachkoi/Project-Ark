using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : Crewmate
{
    public float explorationPointsPerCycle = 10f;

    public Navigator(string crewmateName) : base(crewmateName)
    {
        
    }
    
    public override void PerformDuty()
    {
        base.PerformDuty();

        if (ResourcesManager.Instance.foodSupply >= foodConsumedPerCycle &&
            ResourcesManager.Instance.fuelSupply >= fuelConsumedPerCycle)
        {
            ResourcesManager.Instance.AddExploration(explorationPointsPerCycle);
            Debug.Log($"{crewmateName} provided +{explorationPointsPerCycle} exploration points.");
        }
    }
}