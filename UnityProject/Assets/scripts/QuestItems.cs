using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItems : MonoBehaviour
{
    GameObject gem;
    string gemTag;

    private void Start()
    {
        gem = this.GetComponent<GameObject>();
        gemTag = this.tag;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUp();
        }
    }

    void pickUp()
    {
        Debug.Log(gameManager.instance.questItems.Contains("GemQuest"));
        gameManager.instance.questItems.Add(gemTag);
        Destroy(gameObject);
    }
}
