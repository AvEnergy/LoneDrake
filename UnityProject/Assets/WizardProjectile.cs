using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody RB;

    [SerializeField] float speed;
    [SerializeField] int damage;
    [SerializeField] float destroyTime;

    bool hitHappened;
    // Start is called before the first frame update
    void Start()
    {
        RB.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        iDamage other_dmg = other.GetComponent<iDamage>();
        if (other_dmg != null && !hitHappened)
        {
            other_dmg.takeDamage(damage);
            hitHappened = true;
        }
        IgnoreDamage dmg = other.GetComponent<IgnoreDamage>();
        if (dmg != null && !hitHappened)
        {
            dmg.IgnoreDamage(damage);
            hitHappened = true;
        }

        Destroy(gameObject);
    }
}
