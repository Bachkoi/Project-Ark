using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Crewmate
{
    public float gearBonus = 7f;

    public Engineer(string crewmateName) : base(crewmateName)
    {
        
    }
    
    public override void PerformDuty()
    {
        base.PerformDuty();
        ResourcesManager.Instance.AddGears(gearBonus);
        Debug.Log($"{crewmateName} provides +{gearBonus} gears.");
    }
}
