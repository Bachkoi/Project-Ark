using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CrewUIManager : MonoBehaviour
{
    [BoxGroup("Main"), SerializeField] private List<CrewButton> crewButtons;
    
    [BoxGroup("Popup"), SerializeField] private GameObject popupPanelContainer;
    [BoxGroup("Popup"), SerializeField] private Image popupPanelImg;
    [BoxGroup("Popup"), SerializeField] private Sprite navigatorPanelSprite, farmerPanelSprite, enforcerPanelSprite, medicPanelSprite, engineerPanelSprite;
    [BoxGroup("Popup"), SerializeField] private CrewIndividualButton crewIndividualBtnPrefab;
    [BoxGroup("Popup"), SerializeField] private Transform crewIndividualsContainer;
    
    void Start()
    {
        foreach (CrewButton crewButton in crewButtons)
        {
            crewButton.Btn.onClick.AddListener(() => ShowPopupPanel(crewButton));
        }
    }

    private void ShowPopupPanel(CrewButton crewButton)
    {
        if (crewButton == null)
            return;
    
        popupPanelContainer.gameObject.SetActive(true);
        popupPanelContainer.transform.localPosition = crewButton.transform.localPosition + new Vector3(crewButton.PopupOffset_x, crewButton.PopupOffset_y, 0);

        foreach (Transform individual in crewIndividualsContainer)
        {
            Destroy(individual.gameObject);
        }

        foreach (var crewmate in Crewmate.AllCrewmates)
        {
            switch (crewButton.Role)
            {
                case "Farmer":
                    if (crewmate is Farmer)
                    {
                        CrewIndividualButton button = Instantiate(crewIndividualBtnPrefab, crewIndividualsContainer);
                        button.UpdateUI(crewmate);
                    }
                    popupPanelImg.sprite = farmerPanelSprite;
                    break;
                case "Enforcer":
                    if (crewmate is Enforcer)
                    {
                        CrewIndividualButton button = Instantiate(crewIndividualBtnPrefab, crewIndividualsContainer);
                        button.UpdateUI(crewmate);
                    }
                    popupPanelImg.sprite = enforcerPanelSprite;
                    break;
                case "Engineer":
                    if (crewmate is Engineer)
                    {
                        CrewIndividualButton button = Instantiate(crewIndividualBtnPrefab, crewIndividualsContainer);
                        button.UpdateUI(crewmate);
                    }
                    popupPanelImg.sprite = engineerPanelSprite;
                    break;
                case "Medic":
                    if (crewmate is Medic)
                    {
                        CrewIndividualButton button = Instantiate(crewIndividualBtnPrefab, crewIndividualsContainer);
                        button.UpdateUI(crewmate);
                    }
                    popupPanelImg.sprite = medicPanelSprite;
                    break;
                case "Navigator":
                    if (crewmate is Navigator)
                    {
                        CrewIndividualButton button = Instantiate(crewIndividualBtnPrefab, crewIndividualsContainer);
                        button.UpdateUI(crewmate);
                    }
                    popupPanelImg.sprite = navigatorPanelSprite;
                    break;
            }
            
        }
    }
}
