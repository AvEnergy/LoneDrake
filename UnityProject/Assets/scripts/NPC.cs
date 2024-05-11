using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;
using System.Diagnostics.Contracts;

public class NPC : MonoBehaviour
{
    [SerializeField] GameObject speechBubble;
    [SerializeField] Transform model;
    [SerializeField] TextWriter textWriter;
    [SerializeField] TextMeshPro message;
    bool pauseText;
    bool playerInRange;
    private int currPhrase;
   
    // Start is called before the first frame update
    void Start()
    {
        currPhrase = 0;
        playerInRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            faceTarget();
            if(!pauseText)
            {
                StartCoroutine(startTalking());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            speechBubble.SetActive(true);
            playerInRange = true;
        }
    }

    IEnumerator startTalking()
    {
        pauseText = true;
        message.text = phrase(currPhrase);
        yield return new WaitForSeconds(3);
        currPhrase++;
        pauseText = false;
    }
    private string phrase(int Phrase)
    {
        switch (Phrase)
        {
            case 0: return "Hello there!";
            case 1: return "My name is Capsule";
            case 2: return "I don't think we've ever met.";
        }
        speechBubble.SetActive(false);
        return null;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(gameManager.instance.player.transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);
    }

}
