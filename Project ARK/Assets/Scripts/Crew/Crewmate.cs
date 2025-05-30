using UnityEngine;

public class Crewmate : MonoBehaviour
{
    public string crewmateName;
    public float health;

    public float foodConsumedPerCycle = 1f;
    public float fuelConsumedPerCycle = 0.5f;
    public float resourcesConsumed;

    public virtual void PerformDuty()
    {
        bool hasFood = ResourcesManager.Instance.ConsumeFood(foodConsumedPerCycle);
        bool hasFuel = ResourcesManager.Instance.ConsumeFuel(fuelConsumedPerCycle);

        if (!hasFood || !hasFuel)
        {
            Debug.Log($"{crewmateName} cannot perform duty due to lack of food or fuel.");
            return;
        }

        Debug.Log($"{crewmateName} is performing their duty.");
    }
}