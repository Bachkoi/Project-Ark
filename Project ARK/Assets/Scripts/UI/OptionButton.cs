using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private int index;
    
    [SerializeField] protected float popupOffset_x, popupOffset_y;
    
    //getters & setters
    public int Index {get=>index;}
    public Button Btn {get=>btn;}
    public float PopupOffset_x {get=>popupOffset_x;}
    public float PopupOffset_y {get=>popupOffset_y;}
}
