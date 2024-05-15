using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField]int rotSpeed;
    [SerializeField] AudioSource aud;

    void Update()
    {
       transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            aud.Play();
            Pickup();
            
        }
    }

    public void Pickup()
    {
        int amount = Random.Range(25, 60);
        gameManager.instance.playerScript.GetHP(amount);
        Destroy(gameObject);
    }
}
