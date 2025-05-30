using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigationPanelOption : MonoBehaviour
{
    // Establish necessary fields
    [BoxGroup("ImageElements")] public Image staticImage;
    [BoxGroup("ImageElements")] public Image selectedImage;
    
    [BoxGroup("TextElements")] public TextMeshProUGUI costTMP;
    [BoxGroup("TextElements")] public TextMeshProUGUI resourceBenefitsTMP;

    [BoxGroup("PlanetFunctionFields")] public int cost;
    [BoxGroup("PlanetFunctionFields")] public Dictionary<ResourcesManager.ResourceType, float> resourceBenefits;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        selectedImage.gameObject.SetActive(!selectedImage.gameObject.activeSelf);
    }
}
