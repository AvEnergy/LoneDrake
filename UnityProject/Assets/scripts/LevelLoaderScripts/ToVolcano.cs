using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToVolcano : MonoBehaviour
{
    public Animator transition;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PersistantData.XpToKeep = gameManager.instance.XP;
            PersistantData.LevelToKeep = gameManager.instance.currLvl;
            PersistantData.skillPointToKeep = gameManager.instance.skillPoint;
          
            LoadNextlevel();
        }
    }

    public void LoadNextlevel()
    {
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
       transition.SetTrigger("Start");
       yield return new WaitForSeconds(1f);
       SceneManager.LoadScene(levelIndex);
    }
}
