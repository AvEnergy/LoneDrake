using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField]int rotSpeed;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip healSound;

    void Update()
    {
       transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup();            
        }
    }

    public void Pickup()
    {
        int amount = Random.Range(25, 60);
        if (healSound != null)
        {
            aud.PlayOneShot(healSound);
        }
        else
        {
            Debug.Log("NO PICKUP SOUND");
        }
        gameManager.instance.playerScript.GetHP(amount);
        Destroy(gameObject);
    }
}
