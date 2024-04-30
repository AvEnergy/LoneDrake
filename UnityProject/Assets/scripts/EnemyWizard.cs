using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWizard : MonoBehaviour, iDamage
{


    [Header("--------AI STATS--------")]

    [SerializeField] int hp;
    [SerializeField] int speed;
    [SerializeField] int facetargetSpeed;

    [Header("--------Shooting STATS--------")]
    [SerializeField] bool CanShootAttack;
    [SerializeField] float shootRate;

    [Header("-------Melee Stats------")]
    [SerializeField] bool CanMeleeAttack;
    [SerializeField] int meleeDmg;
    [SerializeField] float meleeDist;
    [SerializeField] float cooldownTime;
    [SerializeField] int viewCone;

    [Header("-------Game Objects------")]
    [SerializeField] NavMeshAgent agent;
    public GameObject playertemp;
    [SerializeField] Renderer model;
    [SerializeField] GameObject FireBall;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;

    bool aggro;
    bool isShooting;
    bool meleeAttack;
    bool playerinRange;
    float angleToPlayer;
    float stoppingDistOrig;
    Vector3 startingPos;
    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        aggro = false;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp > 0)
        {
            if (playerinRange && canSeePlayer())
            {
                aggro = true;
            }
            if (aggro)
            {
                anim.SetTrigger("idle_combat");
                movement();
            }
        }
    }
    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, headPos.position.y, playerDir.z), transform.forward);
        Debug.Log(angleToPlayer);
        agent.SetDestination(gameManager.instance.player.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            Debug.Log(hit.transform.name);
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrig;
                if (!isShooting)
                    StartCoroutine(shootThem());

                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        faceTarget();
                    }
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }


    public void movement()
    {
        if (playerinRange)
        {
            playerDir = gameManager.instance.player.transform.position - transform.position;
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (!isShooting && CanShootAttack)
            {
                StartCoroutine(shootThem());
            }
            if (!meleeAttack && CanMeleeAttack)
            {
                Attack();
            }
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
    }

    //Checks if player is inside the collider 
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerinRange = true;
        }
    }


    //checks if player is outside the collider
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerinRange = false;
        }
    }


    //Enemy faces the player
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facetargetSpeed);
    }
    public void takeDamage(int amount)
    {
        hp -= amount;
        anim.SetTrigger("damage_001");
        if (hp > 0)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
        if (hp <= 0)
        {
            aggro = false;
            stoppingDistOrig = 100;
            anim.SetTrigger("dead");
        }
    }

    IEnumerator shootThem()
    {
        isShooting = true;

        Instantiate(FireBall, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    //cooldown when enemy deals melee damage
    IEnumerator cooldown()
    {
        meleeAttack = true;
        yield return new WaitForSeconds(cooldownTime);
        meleeAttack = false;
    }

    public void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, meleeDist))
        {
            if (hit.collider.CompareTag("Player"))
            {
                iDamage dmg = hit.collider.GetComponent<iDamage>();
                if (hit.transform != transform && dmg != null)
                {

                    dmg.takeDamage(meleeDmg);
                    StartCoroutine(cooldown());
                }
            }
        }
    }
}
