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
        updatedPrompt = prompt;
        if (prompt.Contains("{1}"))
        {
            updatedPrompt = updatedPrompt.Replace("{1}", shipStats);
        }

        if (prompt.Contains("{2}"))
        {
            updatedPrompt = updatedPrompt.Replace("{2}", nextTimeStats);
        }

        if (prompt.Contains("{3}"))
        {
            updatedPrompt = updatedPrompt.Replace("{3}", remainingCrew);
        }

        if (prompt.Contains("{4}"))
        {
            string sentence = "Actions I have already taken: ";
            if (performedOptions.Count > 0)
            {
                
            }
            else
            {
                sentence = "";
            }
            updatedPrompt = prompt.Replace("{4}", sentence);
        }
        
        if (prompt.Contains("{5}"))
        {
            updatedPrompt = updatedPrompt.Replace("{5}", sanity);
        }
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