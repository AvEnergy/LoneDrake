using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSound : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip health_sound;

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            aud.PlayOneShot(health_sound);
        }
        Destroy(gameObject, 1f);
    }

}
