using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ARKOptionGenerator : MonoBehaviour
{
    [SerializeField] private string prompt;
    [SerializeField, ReadOnly] private string updatedPrompt;
    
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
        
        
        
    }
}
