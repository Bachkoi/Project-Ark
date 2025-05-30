using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    public float explorationPoints;
    public float securityLevel;
    public float healthPoints;
    public float foodSupply;
    public float gears;
    public float fuelSupply;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddExploration(float amount) => explorationPoints += amount;
    public void AddSecurity(float amount) => securityLevel += amount;
    public void AddHealth(float amount) => healthPoints += amount;
    public void AddFood(float amount) => foodSupply += amount;
    public void AddGears(float amount) => gears += amount;
    public void AddFuel(float amount) => fuelSupply += amount;

    public bool ConsumeFood(float amount)
    {
        if (foodSupply >= amount)
        {
            foodSupply -= amount;
            return true;
        }
        Debug.LogWarning("Not enough food!");
        return false;
    }

    public bool ConsumeFuel(float amount)
    {
        if (fuelSupply >= amount)
        {
            fuelSupply -= amount;
            return true;
        }
        Debug.LogWarning("Not enough fuel!");
        return false;
    }

    public void LogResources()
    {
        Debug.Log($"Exploration: {explorationPoints}, Security: {securityLevel}, Health: {healthPoints}, Food: {foodSupply}, Gears: {gears}, Fuel: {fuelSupply}");
    }
}