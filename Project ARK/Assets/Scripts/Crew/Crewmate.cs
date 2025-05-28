using UnityEngine;

public class Crewmate : MonoBehaviour
{
    public string crewmateName;
    public float health;
    public float resourcesConsumed;

    // Virtual method that derived classes can override
    public virtual void PerformDuty()
    {
        Debug.Log($"{crewmateName} is performing their duty.");
    }
}