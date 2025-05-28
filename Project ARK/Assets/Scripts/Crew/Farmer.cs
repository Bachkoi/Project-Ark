using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : Crewmate
{
    public float foodBonus = 15f;

    public override void PerformDuty()
    {
        base.PerformDuty();
        Debug.Log($"{crewmateName} farms and provides +{foodBonus} food.");
    }
}
