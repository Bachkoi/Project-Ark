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

    public void UpdateUI(Crewmate crewmate)
    {
        if (crewmate == null)
            return;

        crewNameTxt.text = crewmate.crewmateName;

        if (crewmate is Farmer)
        {
            headshotImg.sprite = farmerHeadshot;
        }
        else if (crewmate is Engineer)
        {
            headshotImg.sprite = engineerHeadshot;
        }
        else if (crewmate is Enforcer)
        {
            headshotImg.sprite = enforcerHeadshot;
        }
        else if (crewmate is Navigator)
        {
            headshotImg.sprite = navigatorHeadshot;
        }
        else if (crewmate is Medic)
        {
            headshotImg.sprite = medicHeadshot;
        }
    }
}
