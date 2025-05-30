using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : Crewmate
{
    public float foodPerCycle = 15f;

    public Farmer(string crewmateName) : base(crewmateName)
    {
        
    }
    public override void PerformDuty()
    {
        base.PerformDuty();

        if (ResourcesManager.Instance.foodSupply >= foodConsumedPerCycle &&
            ResourcesManager.Instance.fuelSupply >= fuelConsumedPerCycle)
        {
            ResourcesManager.Instance.AddFood(foodPerCycle);
            Debug.Log($"{crewmateName} provided +{foodPerCycle} food.");
        }
    }
}
