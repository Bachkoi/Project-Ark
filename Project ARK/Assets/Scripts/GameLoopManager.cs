using UnityEngine;
using System.Collections.Generic;

public class GameLoopManager : MonoBehaviour
{
    public List<Crewmate> crewmates;
    public float cycleInterval = 5f;

    private void Start()
    {
        InvokeRepeating(nameof(RunCycle), 0f, cycleInterval);
    }

    private void RunCycle()
    {
        Debug.Log("---- New Cycle ----");
        foreach (var crewmate in crewmates)
        {
            crewmate.PerformDuty();
        }

        ResourcesManager.Instance.LogResources();
    }
}