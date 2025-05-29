using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class ArkUIManager : MonoBehaviour
{
    [BoxGroup("Options"), SerializeField] private GameObject optionDisplayContainer;
    [BoxGroup("Buttons"), SerializeField] private List<OptionButton> optionBtns;
    void Start()
    {
        foreach (OptionButton optionBtn in optionBtns)
        {
            optionBtn.Btn.onClick.AddListener(() => ShowOption(optionBtn.Index));
        }
    }
    
    private void ShowOption(int index)
    {
        optionDisplayContainer.SetActive(true);
    }
}
