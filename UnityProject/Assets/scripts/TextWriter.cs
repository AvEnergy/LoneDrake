using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour
{
    private Text uiText;
    private string textToWrite;
    private int characterIndex;
    private float timeParChar;
    private float timer;

    public void addWriter(Text uiText, string textToWrite, float timePerChar)
    {
        this.uiText = uiText;
        this.textToWrite = textToWrite;
        this.timeParChar = timePerChar;
        characterIndex = 0;
    }

    private void Update()
    {
        if (uiText != null)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                timer += timeParChar;
                characterIndex++;
                uiText.text = textToWrite.Substring(0, characterIndex);
            }
        }
    }
}
