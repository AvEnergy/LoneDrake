using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToForest : MonoBehaviour
{
    public Animator transition;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PersistantData.XpToKeep = gameManager.instance.XP;
            PersistantData.LevelToKeep = gameManager.instance.currLvl;
            PersistantData.skillPointToKeep = gameManager.instance.skillPoint;
            PersistantData.skillUnlockedCount = SkillManager.instance.count;
            LoadNextlevel();
        }
    }

    public void LoadNextlevel()
    {
        StartCoroutine(LoadLevel(2));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
       transition.SetTrigger("Start");
       yield return new WaitForSeconds(1f);
       SceneManager.LoadScene(levelIndex);
    }
}
