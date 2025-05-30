using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetaryExplorationManager : MonoBehaviour
{
    // Establish Necessary fields
    public List<NavigationPanelOption> options = new List<NavigationPanelOption>();
    public NavigationPanelOption selectedOption;

    public List<Sprite> planetImages = new List<Sprite>();
    
    public GameObject planetPrefab;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExplorePlanet()
    {
        if (selectedOption != null && ResourcesManager.Instance.securityLevel - selectedOption.cost > 0)
        {
            foreach (var resource in selectedOption.resourceBenefits)
            {
                switch (resource.Key)
                {
                    case ResourcesManager.ResourceType.Exploration:
                        ResourcesManager.Instance.AddExploration(resource.Value);
                        break;
                    case ResourcesManager.ResourceType.Food:
                        ResourcesManager.Instance.AddFood(resource.Value);
                        break;
                    case ResourcesManager.ResourceType.Gears:
                        ResourcesManager.Instance.AddGears(resource.Value);
                        break;
                    case ResourcesManager.ResourceType.Health:
                        ResourcesManager.Instance.AddHealth(resource.Value);
                        break;
                    case ResourcesManager.ResourceType.Security:
                        ResourcesManager.Instance.AddSecurity(resource.Value);
                        break;
                }
            }
        }
    }

    public void GeneratePlanets()
    {
        if (options.Count < 3)
        {
            // Generate up until 3
        }
        
        
    }
}
