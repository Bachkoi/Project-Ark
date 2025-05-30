using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : Crewmate
{
    public float foodBonus = 15f;

    public Farmer(string crewmateName) : base(crewmateName)
    {
        
    }
    public override void PerformDuty()
    {
        base.PerformDuty();
        ResourcesManager.Instance.AddFood(foodBonus);
        Debug.Log($"{crewmateName} provides +{foodBonus} food.");
    }
}
