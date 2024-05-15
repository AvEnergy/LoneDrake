using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public int playerlvl;

    [Header("-----SkillTree UI------")]
    [SerializeField] public GameObject skillMenuActive;
    [SerializeField] GameObject skillMenu;


    [Header("--------Skill Tree Elements-------")]

    [SerializeField] string[] skillNames;
    [SerializeField] GameObject skilltree;
    public List<int> level_To_Unlock;
    public List<int> time_Duration;
    public List<SetSkills> skills;

    public int skillpoints;

    public Button currentButton;
    
    public bool displayOn;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        foreach(var skill in skilltree.GetComponentsInChildren<SetSkills>()) 
        { 
            skills.Add(skill); 
        }

        SetLevelUnlock();

        skillpoints = 1;
        playerlvl = 10;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.M))
        {
            if(skillMenuActive == null)
            {
                SkillMenuOn();
            }
        }
    }

    public void SkillMenuOn()
    {
        displayOn = true;
        skillMenu.SetActive(displayOn);
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameManager.instance.SkillTreeButtonSelected);
        Cursor.lockState = CursorLockMode.Confined;
        skillMenuActive = skillMenu;
    }

    public void SkillMenuOff()
    {
        displayOn = false;
        skillMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        skillMenuActive = null;
    }

    public void SetLevelUnlock()
    {
        level_To_Unlock = new List<int>() { 1, 5, 10, 15, 20};
    }
    
}
