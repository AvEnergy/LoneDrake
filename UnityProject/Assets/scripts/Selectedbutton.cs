using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selectedbutton : MonoBehaviour
{
    public GameObject button; //Assign a button for each menu (ex. for main menu assign StartGame button, for optionsMenu assign MasterSlider button)
    
    void OnEnable()
    {
        SetButton();
    }
    void Start()
    {
        SetButton();
    }
    public void SetButton()
    {
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(button);
        }
    }
}
