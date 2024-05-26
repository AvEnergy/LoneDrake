using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HeatPickup : MonoBehaviour
{
    [SerializeField][Range(0, 100)] int heatAmount;
    [SerializeField] AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            pickUp();
        }
    }

    void pickUp()
    {
        gameManager.instance.playerScript.pickUpListener.PlayOneShot(collectSound);
        gameManager.instance.playerScript.GetHeat(heatAmount);
        Destroy(gameObject);
    }
}
