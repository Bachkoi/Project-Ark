using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [BoxGroup("Setting"), SerializeField] private GameObject settingPanel;
    [BoxGroup("Buttons"), SerializeField] private Button arkBtn, settingBtn;

    public static event Action onClickArkBtn;
    
    void Start()
    {
        arkBtn?.onClick.AddListener(OpenArkPanel);
        settingBtn?.onClick.AddListener(OpenSettingPanel);

        
    }

    private void OnDisable()
    {
        arkBtn?.onClick.RemoveListener(OpenArkPanel);
        settingBtn?.onClick.RemoveListener(OpenSettingPanel);
    }

    private void OpenArkPanel()
    {
        onClickArkBtn?.Invoke();
    }

    private void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    
}
