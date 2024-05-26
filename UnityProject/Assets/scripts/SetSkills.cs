using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetSkills : MonoBehaviour
{
    public GameObject player;
    public playerController playerscript;
    

    //Enum created as a way to determine a current state or level
    public enum LevelRequired 
    {
        one,
        two,
        three, 
        four,
        five,
    };
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerscript = player.GetComponent<playerController>();
    }
    //Debug to check for the name of the gameObject when OnClick is called for the function below this.

    //Main function to be called when OnClick() is called. 
    public void unLockSkill()
    {
        
        //Iterates throught each element in the skills list.
        foreach (var skill in SkillManager.instance.skills)
        {
            //Checks to see if the name of element in skill matches the curent name of the object clicked.
            if (skill.name == gameObject.name)
            {
                //Executed a method if true.
                DefineSkill(gameObject.name);
            }
        }

        
    }
    public bool highLighted_cursor()
    {
        return EventSystem.current.IsPointerOverGameObject();

    }

    //Function to preform certains skills when the button is clicked (Trying to work this with Key Inputs)
    public void DefineSkill(string name)
    {
        //switch is used to preform certain cases based on the string for each case such as Invincibility(player doesn't take damage).
        switch(name)
        {
            //We check by accesing the skillpoint in gamaManger to see if they can unlock/upgrade the skill.
            //We also Acess the player level and compare it to the list of Level_To_Unlock the idea is to have a broad list to control the level require to unlock.
            //The list takes the index of the enum values "Level Required" a cast of int is needed since the type is still enum. This result in the value turning into the value of 0 for .One.
            //If all checks are true then we decrement the skillpoint and get change the color to green of the button by Calling GetCompenent of Image.
            case "Invincibility":
                
                if(gameManager.instance.skillPoint >= 0 && gameManager.instance.currLvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.one])
                {
                    gameManager.instance.skillPoint--;
                    GetComponent<Image>().color = Color.green;
                    SkillManager.instance.skillsUnlocked.Add(gameObject.name);
                    playerscript.ActivateIMMORTAL();

                }
                break;

            case "DoubleJump":
                if(gameManager.instance.skillPoint >= 2 && gameManager.instance.currLvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.two])
                {
                    gameManager.instance.skillPoint -= 2;
                    GetComponent<Image>().color = Color.green;
                    SkillManager.instance.skillsUnlocked.Add(gameObject.name);
                    playerscript.SetInvicible(true);
                }
                break;

            case "AoE":
                if (gameManager.instance.skillPoint >= 2 && gameManager.instance.currLvl >= SkillManager.instance.level_To_Unlock[(int)LevelRequired.three])
                {
                    gameManager.instance.skillPoint -= 2;
                    GetComponent<Image>().color = Color.green;
                    SkillManager.instance.skillsUnlocked.Add(gameObject.name);
                    playerscript.ActivateIMMORTAL();
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
                    SkillManager.instance.skillsUnlocked.Add(gameObject.name);
                    gameManager.instance.fireBar.gameObject.SetActive(true);
                    playerscript.SetInvicible(true);
                   
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



    //Function to called if the checks are false for each case (implemented in default but seems unnecessary and could be used for else{} instead.
    IEnumerator Locked()
    {
        GetComponent<Image>().color= Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Image>().color= Color.white;
    }
    
  

   
}