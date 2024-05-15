using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSkills : MonoBehaviour
{
    
    public enum LevelRequired 
    {
        one,
        five,
        ten, 
        fifteen,
        twenty,
    };
    public void GetName()
    {
        Debug.Log(gameObject.name);
    }

    public void unLockSkill()
    {
        foreach (var skill in SkillManager.instance.skills)
        {
            if (skill.name == gameObject.name)
            {
                DefineSkill(gameObject.name);
            }
        }
    }
    public void DefineSkill(string name)
    {
        switch(name)
        {
            case "Invincibility":
                if(SkillManager.instance.skillpoints > 0 && SkillManager.instance.playerlvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.one])
                {
                    SkillManager.instance.skillpoints--;
                    GetComponent<Image>().color = Color.green;
                    //StartCoroutine(gameManager.instance.playerScript.duration());           
                }
                break;

            case "DoubleJump":
                if(SkillManager.instance.playerlvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.five])
                {
                    GetComponent<Image>().color = Color.blue;
                }
                break;

            case "AoE":
                break;

            case "Dash":
                break;

            case "Flame":
                SkillManager.instance.skillpoints--;
                gameManager.instance.playerScript.isFlameThrower = true;
                StartCoroutine(duration(3));
                gameManager.instance.playerScript.isFlameThrower = false;
                break;
            case "Glide":
                break;
            default:
                StartCoroutine(Locked());
                break;
        }
    }

    IEnumerator Locked()
    {
        GetComponent<Image>().color= Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Image>().color= Color.white;
    }
    IEnumerator duration(int amount)
    {
        GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(amount);
        GetComponent<Image>().color = Color.white;
    }

}