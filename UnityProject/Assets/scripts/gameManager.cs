using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPaused;
    [SerializeField] GameObject menuWinner;
    [SerializeField] GameObject menuLoser;
    public Image playerHPBar;

    public GameObject player;

    public bool isPaused;
    int enemyCount;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
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
    public void updateGameGoal(int amount)
    {
        enemyCount += amount;

        if (enemyCount <= 0)
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
}
