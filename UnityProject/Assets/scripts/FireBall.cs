using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FireBall : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] float speed;
    [SerializeField] int damage;
    [SerializeField] int destroyTime;

    bool hitHappened;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.CompareTag("Player"))
        {
            return;
        }
        iDamage other_dmg = other.GetComponent<iDamage>();
        if (other_dmg != null && !hitHappened && !other.CompareTag("Player"))
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
