using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ARKOptionGenerator : MonoBehaviour
{
    [SerializeField, TextArea(10, 10)] private string prompt;
    [SerializeField, ReadOnly, TextArea(10, 10)] private string updatedPrompt;
    
    [BoxGroup("Test"), SerializeField, TextArea(7, 7)] private string shipStats, nextTimeStats, remainingCrew;

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
        // UnityToGemini.Instance.SendRequest(updatedPrompt, GeminiRequestType.Option);
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
            updatedPrompt = updatedPrompt.Replace("2", nextTimeStats);
        }

        if (prompt.Contains("{3}"))
        {
            updatedPrompt = updatedPrompt.Replace("3", remainingCrew);
        }
    }

    private void UnpackOptionResponse(string rawResponse, GeminiRequestType requestType)
    {
        if (requestType != GeminiRequestType.Option)
            return;
    
        Debug.Log(rawResponse);
        try
        {
            // Parse the raw response into GeminiResponse object
            Backend.GeminiResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<Backend.GeminiResponse>(rawResponse);
        
            if (response != null && response.Candidates != null && response.Candidates.Count > 0)
            {
                // Get the text content from the first candidate
                string optionsJson = response.Candidates[0].Contents.Parts[0].Text;
            
                // Parse the options text into a list of ARKOption objects
                options = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ARKOption>>(optionsJson);
            
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
}