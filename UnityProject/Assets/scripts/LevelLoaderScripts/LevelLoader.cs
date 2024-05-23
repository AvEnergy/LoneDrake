using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    [SerializeField] GameObject settingsPump;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PersistantData.XpToKeep = gameManager.instance.XP;
            PersistantData.LevelToKeep = gameManager.instance.currLvl;
            LoadNextlevel();
        }
    }

    public void LoadNextlevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
       transition.SetTrigger("Start");
       yield return new WaitForSeconds(1f);
       SceneManager.LoadScene(levelIndex);
       settingsPump.SetActive(true);
       settingsPump.SetActive(false);
    }
}
