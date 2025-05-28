using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    public float explorationPoints;
    public float securityLevel;
    public float healthPoints;
    public float foodSupply;
    public float gears;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persist across scenes
    }

    public void AddExploration(float amount) => explorationPoints += amount;
    public void AddSecurity(float amount) => securityLevel += amount;
    public void AddHealth(float amount) => healthPoints += amount;
    public void AddFood(float amount) => foodSupply += amount;
    public void AddGears(float amount) => gears += amount;

    public void LogResources()
    {
        Debug.Log($"Exploration: {explorationPoints}, Security: {securityLevel}, Health: {healthPoints}, Food: {foodSupply}, Gears: {gears}");
    }
}