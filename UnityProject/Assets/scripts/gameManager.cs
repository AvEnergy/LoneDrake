using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [Header("-------XPtracking------")]
    [SerializeField] AudioSource levelUpSound;
    public Image XPBar;
    public TMP_Text levelText;
    public int currLvl;
    public int XP;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject menuPaused;
    [SerializeField] GameObject menuWinner;
    [SerializeField] GameObject menuLoser;
    public GameObject menuCheckPoint;
    public TMP_Text objText;
    public GameObject playerIsHit;
    public Image playerHPBar;
    public Image fireBar;
    public Image bossHp;


    public GameObject player;
    public GameObject playerSpawnPos;
    public playerController playerScript;

    public bool isPaused;
    public int enemyCount;
    public bool bossNotKilled;

    

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerSpawnPos = GameObject.FindWithTag("Player Spawn POS");
        playerScript = player.GetComponent<playerController>();
        bossNotKilled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPaused;
            menuActive.SetActive(isPaused);
        }
        levelObjective();
    }
    //Stops the game, gives player back mouse. Called when 
    public void statePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    //Using buttons in the menus calls this function to unpause the game. Located in buttonFunctions
    public void stateUnPaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }
    //Checks to see if all enemys are dead, if yes, you win.
    public void updateGameGoal()
    {
        if (bossNotKilled == false)
        {
            statePaused();
            menuActive = menuWinner;
            menuActive.SetActive(isPaused);
        }
    }
    //Called if player loses all their HP.
    public void youLoser()
    {
        statePaused();
        menuActive = menuLoser;
        menuActive.SetActive(menuLoser);
    }
  
    public void levelObjective()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            objText.text = "Head to the Dwarf Fortress.";
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if(!gameManager.instance.playerScript.hasKey)
            {
                objText.text = "Find the key.";
            }
            else
            {
                objText.text = "Enter the fortess.";
            }

        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            objText.text = "Kill the wizard and save mom.";
        }
    }

    public void givePlayerXP(int XPamount)
    {
        XP += XPamount;
        if(XP >= 100)
        {
            XP -= 100;
            currLvl++;
            levelText.text = currLvl.ToString();
            levelUpSound.Play();
        }
        playerScript.updatePlayerUI();
    }
}
