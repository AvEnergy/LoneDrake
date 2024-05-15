using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonFunc : MonoBehaviour
{
   public void TurnOn()
    {
        SkillManager.instance.SkillMenuOn();
        
    }
   public void TurnOff()
    {
        SkillManager.instance.SkillMenuOff();
    }
    public void OnButtonClick(GameObject button)
    {
        Debug.Log("Button name: " + button.name);
    }
}
