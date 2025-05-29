using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CrewUIManager : MonoBehaviour
{
    [BoxGroup("Main"), SerializeField] private List<CrewButton> crewButtons;
    
    [BoxGroup("Popup"), SerializeField] private GameObject popupPanelContainer;
    [BoxGroup("Popup"), SerializeField] private Sprite navigatorHeadshot, medicHeadshot, engeneerHeadshot, farmerHeadshot, enforcerHeadshot;
    [BoxGroup("Popup"), SerializeField] private CrewIndividualButton crewIndividualBtnPrefab;

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
    }
}
