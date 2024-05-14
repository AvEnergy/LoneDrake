using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItems : MonoBehaviour
{
    public GameObject gem;
    public string gemTag;

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
        Debug.Log(gameManager.instance.questItems.Count);
        gameManager.instance.questItems.Add(gemTag);
        Destroy(gameObject);
    }
}
