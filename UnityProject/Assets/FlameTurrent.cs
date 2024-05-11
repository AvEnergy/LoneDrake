using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTurrent : MonoBehaviour
{
    [SerializeField] int damage;

    bool doDamage;
    iDamage dmg;

    // Update is called once per frame
    void Update()
    {
        if (dmg != null && doDamage)
        {
            dmg.takeDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        dmg = other.GetComponent<iDamage>();
        doDamage = true;
    }

    private void OnTriggerExit(Collider other)
    {
        dmg = null;
        doDamage = false;
    }
}
