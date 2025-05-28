using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enforcer : Crewmate
{
    public float securityBonus = 8f;

    public override void PerformDuty()
    {
        base.PerformDuty();
        Debug.Log($"{crewmateName} guards and provides +{securityBonus} security.");
    }
}
