using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrackingPoints : MonoBehaviour
{

    public TMP_Text text_Update;

    // Start is called before the first frame update
    void Start()
    {
        GetSkillSpoint();
        GetSkillUnlocked();
    }

    void Update()
    {
        switch(text_Update.name)
        {
            case "skillpoint":
                GetSkillSpoint();
                break;
            case "skillunlocked":
                GetSkillUnlocked();
                break;
        }
    }
    public void GetSkillSpoint()
    {
        text_Update.text = gameManager.instance.skillPoint.ToString();
    }
    public void GetSkillUnlocked()
    {
        text_Update.text = SkillManager.instance.skillsUnlocked.Count.ToString();
    }
}
