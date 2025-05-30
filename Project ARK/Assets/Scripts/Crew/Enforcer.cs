using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enforcer : Crewmate
{
    public float securityBonus = 8f;

    public Enforcer(string crewmateName) : base(crewmateName)
    {
        
    }
    
    public override void PerformDuty()
    {
        base.PerformDuty();
        ResourcesManager.Instance.AddSecurity(securityBonus);
        Debug.Log($"{crewmateName} provides +{securityBonus} security.");
    }
}
