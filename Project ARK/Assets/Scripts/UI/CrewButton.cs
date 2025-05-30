using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewButton : MonoBehaviour
{
    [SerializeField] protected Button m_button;
    [SerializeField] protected float popupOffset_x, popupOffset_y;

    [SerializeField] protected string role;
    //getters & setters
    public Button Btn { get=>m_button; }
    public float PopupOffset_x { get => popupOffset_x;}
    public float PopupOffset_y { get => popupOffset_y;}
    public string Role { get => role;}
}
