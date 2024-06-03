using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DisplayMessage : MonoBehaviour
{
    public GameObject DISPLAY_MESSAGE;
    public TMP_Text text;

    private void Start()
    {
        
    }
    public void OnGameObject()
    {
        foreach (var skill_name in SkillManager.instance.skills)
        {
            if (SkillManager.instance.skills == null)
            {
                return;
            }
            else
            {
                if (gameObject.name.Contains(skill_name.name))
                {
                    switch(skill_name.name)
                    {
                        case "Invincibility":
                            text.text = "Player Takes No Damage for 5 seconds " +
                                "then recieves a cooldown";
                            break;
                        case "DoubleJump":
                            text.text = "Player is able to Double Jump";
                            break;
                        case "AoE":
                            text.text = "Player deals Area of Deamage to enemies";
                            break;
                        case "Dash":
                            text.text = "Player can Dash for 5 seconds before a cooldown";
                            break;
                        case "Flame":
                            text.text = "Player unlocks the ability to shoot fire like a flamwthrower";
                            break;
                        case "Glide":
                            text.text = "Player glides in the air for 5 seconds then cooldown"; 
                            break;
                    }
                }
            }
        }
        DISPLAY_MESSAGE.SetActive(true);
    }

  public void OnGameObjectExit()
    {
        DISPLAY_MESSAGE.SetActive(false);
    }
}
