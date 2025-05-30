using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : Crewmate
{
    public float explorationPointsBonus = 10f;

    public Navigator(string crewmateName) : base(crewmateName)
    {
        
    }
    
    public override void PerformDuty()
    {
        base.PerformDuty();
        ResourcesManager.Instance.AddExploration(explorationPointsBonus);
        Debug.Log($"{crewmateName} provides +{explorationPointsBonus} exploration points.");
    }
}