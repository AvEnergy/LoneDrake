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
        two,
        three, 
        four,
        five,
    };
    public void GetName()
    {
        Debug.Log(gameObject.name);
    }

    public void unLockSkill()
    {
        GetName();
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
                if(gameManager.instance.skillPoint >= 0 && gameManager.instance.currLvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.one])
                {
                    gameManager.instance.skillPoint--;
                    GetComponent<Image>().color = Color.blue; 
                }
                break;

            case "DoubleJump":
                if(gameManager.instance.skillPoint >= 2 && gameManager.instance.currLvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.two])
                {
                    gameManager.instance.skillPoint -= 2;
                    GetComponent<Image>().color = Color.green;
                }
                break;

            case "AoE":
                if (gameManager.instance.skillPoint >= 2 && gameManager.instance.currLvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.three])
                {
                    gameManager.instance.skillPoint -= 2;
                    GetComponent<Image>().color = Color.green;
                }
                break;

            case "Dash":
                if (gameManager.instance.skillPoint >= 2 && gameManager.instance.currLvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.four])
                {
                    gameManager.instance.skillPoint -= 2;
                    GetComponent<Image>().color = Color.green;
                }
                break;
            case "Flame":
                if (gameManager.instance.skillPoint >= 3 && gameManager.instance.currLvl>= SkillManager.instance.level_To_Unlock[(int)LevelRequired.five])
                {
                    gameManager.instance.skillPoint -= 3;
                    GetComponent<Image>().color = Color.green;
                }
                break;
            case "Glide":
                if (gameManager.instance.skillPoint >= 3 && gameManager.instance.currLvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.five])
                {
                    gameManager.instance.skillPoint -= 3;
                    GetComponent<Image>().color = Color.green;
                }
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
    
   
}