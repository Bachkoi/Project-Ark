using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Crewmate
{
    public float gearBonus = 7f;

    public override void PerformDuty()
    {
        base.PerformDuty();
        Debug.Log($"{crewmateName} maintains systems and provides +{gearBonus} gears.");
    }
}
