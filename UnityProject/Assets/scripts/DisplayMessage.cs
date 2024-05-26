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
    

    public void OnGameObject()
    {
        foreach (var skill_name in SkillManager.instance.skillsUnlocked)
        {
            if (SkillManager.instance.skillsUnlocked == null)
            {
                return;
            }
            else
            {
                if (gameObject.name.Contains(skill_name))
                {
                    switch(skill_name)
                    {
                        case "Invincibility":
                            text.text = "Player Takes No Damage for 5 seconds";
                            break;
                        case "DoubleJump":
                            text.text = "Player Takes No Damage for 5 seconds";
                            break;
                        case "AoE":
                            text.text = "Player Takes No Damage for 5 seconds";
                            break;
                        case "Dash":
                            text.text = "Player Takes No Damage for 5 seconds";
                            break;
                        case "Flame":
                            text.text = "Player Takes No Damage for 5 seconds";
                            break;
                        case "Glide":
                            text.text = "Player Takes No Damage for 5 seconds";
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
