using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : Crewmate
{
    public float healingBonus = 12f;

    public Medic(string crewmateName) : base(crewmateName)
    {
        
    }
    
    public override void PerformDuty()
    {
        base.PerformDuty();
        ResourcesManager.Instance.AddHealth(healingBonus);
        Debug.Log($"{crewmateName} provides +{healingBonus} health.");
    }
}
