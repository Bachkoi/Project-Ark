using System;
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
    [BoxGroup("Options"), SerializeField, ReadOnly] private List<ARKOption> arkOptions;
    [BoxGroup("Buttons"), SerializeField] private List<OptionButton> optionBtns;

    private int currentIndex = -1;

    void OnEnable()
    {
        ARKOptionGenerator.onOptionGenerated += UpdateArkOptions;
    }

    private void OnDisable()
    {
        ARKOptionGenerator.onOptionGenerated -= UpdateArkOptions;
    }

    void Start()
    {
        foreach (OptionButton optionBtn in optionBtns)
        {
            optionBtn.Btn.onClick.AddListener(() => ShowOption(optionBtn.Index));
        }
    }

    private void UpdateArkOptions(List<ARKOption> arkOptions)
    {
        this.arkOptions = arkOptions;
    }
    
    private void ShowOption(int index)
    {
        optionDisplayContainer.SetActive(true);
        optionDisplayContainer.transform.localPosition = optionBtns[index].transform.localPosition + new Vector3(optionBtns[index].PopupOffset_x, optionBtns[index].PopupOffset_y, 0);
        if (index == currentIndex)
            return;
        currentIndex = index;
        
        optionDisplayImg.sprite = optionDisplaySprites[index];

        foreach (Transform individualBtn in crewIndividualsContainer)
        {
            Destroy(individualBtn.gameObject);
        }
        
        foreach (var crewmate in arkOptions[index].crewMates)
        {
            CrewIndividualButton crew = Instantiate(crewIndividualBtnPrefab, crewIndividualsContainer);
            crew.UpdateUI(crewmate);
        }
        
    }
}
