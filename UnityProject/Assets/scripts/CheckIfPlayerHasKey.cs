using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfPlayerHasKey : MonoBehaviour
{
    [SerializeField] Collider Wall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager.instance.playerScript.hasKey)
            {
                Destroy(gameObject);
            }
        }
    }
}
