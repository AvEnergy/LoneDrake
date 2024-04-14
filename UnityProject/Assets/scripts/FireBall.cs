using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] float speed;
    [SerializeField] int damage;
    [SerializeField] int destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        iDamage dmg = other.GetComponent<iDamage>();
        if (dmg != null )
        {
            dmg.takeDamage(damage);
        }
        Destroy(gameObject);
    }
}
