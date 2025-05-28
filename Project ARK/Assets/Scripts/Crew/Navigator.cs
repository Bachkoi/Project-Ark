using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : Crewmate
{
    public float explorationPointsBonus = 10f;

    public override void PerformDuty()
    {
        base.PerformDuty();
        Debug.Log($"{crewmateName} explores and provides +{explorationPointsBonus} exploration points.");
    }
}