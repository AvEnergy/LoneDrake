using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : MonoBehaviour
{
    /// <summary>
    /// Easy Script to make sure Mini Boss inflicts Damage, this will be useful not waste time in assigning shootspos or stuff.
    /// All it needs is an empty object created based on the location of the mace or melee type with a sphere collider compenent attched setting isTrigger on
    /// You could make it access the GeneralBossAI elements but I want to avoid that and just implement this simple process to clear up the GeneralBOssAI script.
    /// 
    /// If you feel like this could improved please do so or if you prefer the rayCast then feel free to remove the script and the empty object I created.
    /// </summary>


    //Simplistic collider for General Boss to deal damage similar to the Arrow script
    int count;
    bool canHit;
    [Range(5, 20)][SerializeField] int damage;
    [SerializeField] int cooldowntime;
    // Start is called before the first frame update



    //Mace script is attached to an object (the location of the weapon)
    //Uses an OnTriggerEnter to detect the player Tagged with "Player"
    private void OnTriggerEnter(Collider other)
    {
        count++;                                        //count the amount of times the player entered the collision
        if (other.CompareTag("Player"))
        {
            
            iDamage dmg = other.GetComponent<iDamage>();

            if (dmg != null && !canHit && count == 1)   //A bool to control when to apply damage since Trigger will apply continous damage if the player remains inside the collider.
            {
                dmg.takeDamage(damage);
                StartCoroutine(cooldown());
            }
        }
    }

    //added a variable to control how the damage should be applied, having bool set with the IEnumerator still applys damage since canHit turns true for two seconds dealing double damage.
    private void OnTriggerExit(Collider other)
    {
        count--;
    }

    //Set the bool to true and false relative to the time transition of the mace swingin (in this case its 2)
    IEnumerator cooldown()
    {
        canHit = true;
        yield return new WaitForSeconds(cooldowntime);
        canHit = false;
    }
}