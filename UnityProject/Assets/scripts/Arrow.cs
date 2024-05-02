using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
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
        if (other.isTrigger)
        {
            return;
        }
        iDamage dmg = other.GetComponent<iDamage>();
        if (dmg != null && !hitHappened)
        {
            dmg.takeDamage(damage);
            hitHappened = true;
        }
        Destroy(gameObject);
    }
}
