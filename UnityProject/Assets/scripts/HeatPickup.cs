using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HeatPickup : MonoBehaviour
{
    [SerializeField][Range(0, 100)] int heatAmount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            pickUp();
        }
    }

    void pickUp()
    {
        gameManager.instance.playerScript.GetHeat(heatAmount);
        Destroy(gameObject);
    }
}
