using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ArkUIManager : MonoBehaviour
{
    [BoxGroup("Options"), SerializeField] private GameObject optionDisplayContainer;
    [BoxGroup("Options"), SerializeField] private TextMeshProUGUI optionDisplayTxt;
    [BoxGroup("Options"), SerializeField] private Image optionDisplayImg;
    [BoxGroup("Options"), SerializeField] private List<Sprite> optionDisplaySprites;
    [BoxGroup("Options"), SerializeField, Tooltip("parent transform that crewIndividualBtnPrefab should instantiate beneath")] private Transform crewIndividualsContainer;
    [BoxGroup("Options"), SerializeField] private CrewIndividualButton crewIndividualBtnPrefab;
    [BoxGroup("Buttons"), SerializeField] private List<OptionButton> optionBtns;

    private int currentIndex = -1;
    
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
        if (index == currentIndex)
            return;
        currentIndex = index;
        
        optionDisplayImg.sprite = optionDisplaySprites[index];
        
    }
}
