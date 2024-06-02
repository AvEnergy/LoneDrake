using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] AudioSource mixer;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] GameObject optionsMenu;

    private void Start()
    {
        //StartCoroutine(quickToggleOptions());
    }
    public void playGame()
    {
        StartCoroutine(delayStart());
    }
    IEnumerator delayStart()
    {
        mixer.volume = Mathf.Lerp(1f, 0.01f, 0.5f);
        yield return new WaitForSeconds(1);
        levelLoader.LoadNextlevel();
    }

    IEnumerator quickToggleOptions()
    {
        optionsMenu.SetActive(true);
        yield return new WaitForSeconds(0.001f);
        optionsMenu .SetActive(false);
    }

    public void quitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
