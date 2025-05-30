using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ARKOptionGenerator : MonoBehaviour
{
    [SerializeField, TextArea(10, 10)] private string prompt;
    [SerializeField, TextArea(10, 10)] private string updatedPrompt;
    [SerializeField, ReadOnly] private List<ARKOption> performedOptions = new List<ARKOption>();
    
    [BoxGroup("Test"), SerializeField, TextArea(7, 7)] private string shipStats, nextTimeStats, remainingCrew, sanity;

    [SerializeField, ReadOnly] private List<ARKOption> options;
    
    public static event Action<List<ARKOption>> onOptionGenerated;

    [Button("Generate Test Crewmates")]
    public void GenerateTestCrewmates()
    {
        if (string.IsNullOrEmpty(remainingCrew))
        {
            Debug.LogError("Remaining crew string is empty");
            return;
        }
        
        // Clear existing crewmates if any
        if (Crewmate.AllCrewmates != null)
        {
            foreach (var crewmate in Crewmate.AllCrewmates.ToArray())
            {
                if (crewmate != null && crewmate.gameObject != null)
                {
                    DestroyImmediate(crewmate.gameObject);
                }
            }
        }
        
        // Parse crew members from the string
        string[] crewEntries = remainingCrew.Split(';');
        
        foreach (string entry in crewEntries)
        {
            string trimmedEntry = entry.Trim();
            if (string.IsNullOrEmpty(trimmedEntry))
                continue;
                
            string[] parts = trimmedEntry.Split(',');
            if (parts.Length < 2)
            {
                Debug.LogWarning($"Invalid crew entry format: {trimmedEntry}");
                continue;
            }
            
            string name = parts[0].Trim();
            string role = parts[1].Trim();
            
            // Create the GameObject for this crewmate
            GameObject crewmateObj = new GameObject(name);
            crewmateObj.transform.SetParent(transform);
            
            // Add the appropriate component based on role
            Crewmate crewmate = null;
            
            switch (role.ToLower())
            {
                case "engineer":
                    crewmate = crewmateObj.AddComponent<Engineer>();
                    break;
                case "farmer":
                    crewmate = crewmateObj.AddComponent<Farmer>();
                    break;
                case "enforcer":
                    crewmate = crewmateObj.AddComponent<Enforcer>();
                    break;
                case "medic":
                    crewmate = crewmateObj.AddComponent<Medic>();
                    break;
                case "navigator":
                    crewmate = crewmateObj.AddComponent<Navigator>();
                    break;
                default:
                    crewmate = crewmateObj.AddComponent<Crewmate>();
                    Debug.LogWarning($"Unknown role: {role}, using base Crewmate class");
                    break;
            }
            
            // Set crewmate properties
            if (crewmate != null)
            {
                crewmate.crewmateName = name;
                crewmate.health = 100f;
                crewmate.resourcesConsumed = 1f;
            }
            
            Debug.Log($"Created {role}: {name}");
        }
        
        Debug.Log($"Generated {(Crewmate.AllCrewmates != null ? Crewmate.AllCrewmates.Count : 0)} crewmates");
    }
    
    private void OnEnable()
    {
        UnityToGemini.GeminiResponseCallback += UnpackOptionResponse;
    }

    private void OnDisable()
    {
        UnityToGemini.GeminiResponseCallback -= UnpackOptionResponse;
    }

    void Start()
    {
        UpdatePrompt();
        UnityToGemini.Instance.SendRequest(updatedPrompt, GeminiRequestType.Option);
    }
    
    public void UpdatePrompt()
    {
        Debug.Log("Updating prompt");
        updatedPrompt = prompt;
        if (updatedPrompt.Contains("{1}"))
        {
            Debug.Log("Update 1\n" + updatedPrompt);
            updatedPrompt = updatedPrompt.Replace("{1}", shipStats);
        }

        if (updatedPrompt.Contains("{2}"))
        {
            updatedPrompt = updatedPrompt.Replace("{2}", nextTimeStats);
        }

        if (updatedPrompt.Contains("{3}"))
        {
            updatedPrompt = updatedPrompt.Replace("{3}", remainingCrew);
        }

        if (updatedPrompt.Contains("{4}"))
        {
            string sentence = "Actions I have already taken: ";
            if (performedOptions.Count > 0)
            {
                
            }
            else
            {
                sentence = "";
            }
            updatedPrompt = updatedPrompt.Replace("{4}", sentence);
        }
        
        if (updatedPrompt.Contains("{5}"))
        {
            updatedPrompt = updatedPrompt.Replace("{5}", sanity);
        }
        Debug.Log("Updating prompt\n" + updatedPrompt);
    }

    private void UnpackOptionResponse(string rawResponse, GeminiRequestType requestType)
    {
        if (requestType != GeminiRequestType.Option)
            return;
    
        try
        {
            // Parse the raw response into GeminiResponse object
            Backend.GeminiResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<Backend.GeminiResponse>(rawResponse);
        
            if (response != null && response.Candidates != null && response.Candidates.Count > 0)
            {
                // Get the text content from the first candidate
                string responseText = response.Candidates[0].Contents.Parts[0].Text;
                
                // Extract JSON content from markdown code block if present
                string jsonContent = ExtractJsonFromText(responseText);
                
                if (string.IsNullOrEmpty(jsonContent))
                {
                    Debug.LogError("Could not extract JSON from response text");
                    return;
                }
                
                // Parse the options text into a list of ARKOption objects
                options = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ARKOption>>(jsonContent);

                foreach (var arkOption in options)
                {
                    arkOption.NamesToCrewmates();
                }
                
                onOptionGenerated?.Invoke(options);
                
                Debug.Log($"Successfully unpacked {options.Count} options");
            }
            else
            {
                Debug.LogError("Invalid or empty GeminiResponse structure");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error unpacking option response: {ex.Message}");
        }
    }
    
    private string ExtractJsonFromText(string text)
    {
        // Look for JSON content inside code blocks (```json ... ```)
        int startIndex = text.IndexOf("```json");
        if (startIndex != -1)
        {
            startIndex = text.IndexOf("[", startIndex);
            if (startIndex != -1)
            {
                int endIndex = text.IndexOf("```", startIndex);
                if (endIndex != -1)
                {
                    return text.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }
        }
        
        // If no code block, try to find JSON array directly
        startIndex = text.IndexOf("[");
        if (startIndex != -1)
        {
            int endIndex = text.LastIndexOf("]");
            if (endIndex > startIndex)
            {
                return text.Substring(startIndex, endIndex - startIndex + 1).Trim();
            }
        }
        
        // If we can't find proper JSON, return empty string
        return string.Empty;
    }
}