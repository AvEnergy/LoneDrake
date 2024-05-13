using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using System;

public class NPC : MonoBehaviour
{
    [SerializeField] GameObject speechBubble;
    [SerializeField] Transform model;
    [SerializeField] TextWriter textWriter;
    [SerializeField] TextMeshPro message;
    [SerializeField] NPCTextCreator textCreator;
    bool pauseText;
    bool playerInRange;
    
    private int currPhrase;
    private int currDial;
    private bool questComplete;

    // Start is called before the first frame update
    void Start()
    {
        currPhrase = 0;
        playerInRange = false;
        questComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            faceTarget();
            if(!pauseText)
            {
                StartCoroutine(startTalking(whichdialogueIsOn(currDial)));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currPhrase = 0;
            speechBubble.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            speechBubble.SetActive(false);
        }
    }

    IEnumerator startTalking(List<string> dial)
    {
        pauseText = true;
        message.text = dial[currPhrase];
        yield return new WaitForSeconds(2);
        if (currPhrase + 1 < dial.Count)
        {
            currPhrase++;
        }
        else
        {
            currPhrase = 0;
            if(!questComplete)
                currDial = 1;
            else
                currDial = 2;
        }
        pauseText = false;
    }

    private List<string> whichdialogueIsOn(int num)
    {
        switch(num)
        {
            case 0:
                {
                    return textCreator.greetings;
                }
            case 1: 
                {
                    return textCreator.quest; 
                }
            case 2: 
                {
                    return textCreator.thanks; 
                }
        }
        return null;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(gameManager.instance.player.transform.position - model.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 6);
    }
}
