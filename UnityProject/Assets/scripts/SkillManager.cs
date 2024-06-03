using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [Header("-----Player Variables----")]
    public GameObject player;
    public playerController playerscript;


    [Header("-----SkillTree UI------")]
    [SerializeField] public GameObject skillMenuActive; //Active menu to display.
    [SerializeField] GameObject skillMenu;             //Menu that will be displayed.


    [Header("--------Skill Tree Elements-------")]


    [SerializeField] GameObject skilltree;    //variable that is the parent of the objects in GetCompenentsInChildren.
    public List<int> level_To_Unlock;        //Simple List that is intialized to create levels.
    public List<string> skillsUnlocked;     //SetSkills will use this store the name's of the objects related to the skill that is being unlocked.
    public List<SetSkills> skills;//Used to store objects that have "SetSkills" script.
    public int count;

    [Header("----------------------------------")]

    
    public bool displayOn;

    // Start is called before the first frame update
    void Awake()
    {
        
        if(instance == null)
        {
            instance = this;
        }
        player = GameObject.FindWithTag("Player");
        playerscript = player.GetComponent<playerController>();
        count = skillsUnlocked.Count;

        count = PersistantData.skillUnlockedCount;
        displayOn = false;
    }

    private void Start()
    {
        //Finds any objects that are childs of skilltree variable 
        //containing the script "SetSkills" and adds them to a list called Skills.
        //This should add only the buttons of the skill tree menu since they're the only ones that a SetSkills script attached.


        foreach(var skill in skilltree.GetComponentsInChildren<SetSkills>()) 
        {
            if (skill != null)
            {
                skills.Add(skill);
            }
        }
     
        //Function to initialize a list and set some levels.
        SetLevelUnlock();
    }


    // Update is called once per frame
    void Update()
    {
        //Pressing the M key will open the skill menu. 
        if (Input.GetKeyUp(KeyCode.M))
        {
            displayOn = !displayOn;
        }
        //Checks if SkillMenuActive doesn't have anything. Also Checks if the menuActive doesn't contain anything. Preventing menus from GameManger to open. 
        if (displayOn && gameManager.instance.menuActive == null)
        {
            SkillMenuOn();
        }

        if (!displayOn && gameManager.instance.menuActive == skillMenu)
        {
            SkillMenuOff();
        }
    }

  

    //Function that opens the skill Menu
    public void SkillMenuOn()
    {
        skillMenu.SetActive(displayOn);
        Cursor.visible = true;

        //Set the current object selected to null since in most cases the object could be assigned to another object from other UI's.
        EventSystem.current.SetSelectedGameObject(null);
        //

        //Set the current object selected to the object called "SkillTreeButtonSelected" (Assigned the button, toggle, or whatever interactable you wish to access)"
        EventSystem.current.SetSelectedGameObject(gameManager.instance.SkillTreeButtonSelected);
        //This works to open visually see a button selected whenever the skill Menu is opened.

        Cursor.lockState = CursorLockMode.Confined;
        skillMenuActive = skillMenu;
        gameManager.instance.menuActive = skillMenuActive;
        gameManager.instance.statePaused();
    }


   //Turns of the Skill Menu
    public void SkillMenuOff()
    {
        skillMenu.SetActive(false);
        displayOn = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        skillMenuActive = null;
        gameManager.instance.stateUnPaused();
    }


    public void SetLevelUnlock()
    {
        level_To_Unlock = new List<int>() { 1, 2, 3, 4, 5};
    }
    
}
