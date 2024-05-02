using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] bool itsaKey;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUp();
        }
    }

    void pickUp()
    {
        gameManager.instance.playerScript.hasKey = true;
        Destroy(gameObject);
    }
}
