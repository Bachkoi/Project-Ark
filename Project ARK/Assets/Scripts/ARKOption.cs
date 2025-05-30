using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ARKOption
{
    public List<string> names = new List<string>();
    public string reason = "";

    public List<Crewmate> crewMates = new List<Crewmate>();

    /// <summary>
    /// converting names into crewmates
    /// </summary>
    public void NamesToCrewmates()
    {
        crewMates.Clear();
        foreach (var name in names)
        {
            crewMates.Add(Crewmate.FindCrewmate(name));
        }
    }
}
