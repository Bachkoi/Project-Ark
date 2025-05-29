using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewIndividualButton : MonoBehaviour
{
    [SerializeField] protected Image headshotImg;
    [SerializeField] protected TextMeshProUGUI crewNameTxt;
    
    [Space]
    [SerializeField] protected Sprite farmerHeadshot, engineerHeadshot, enforcerHeadshot, navigatorHeadshot, medicHeadshot;

    public void UpdateUI(Sprite headshot, string crewName)
    {
        headshotImg.sprite = headshot;
        crewNameTxt.text = crewName;
    }
}
