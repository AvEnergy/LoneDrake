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

    [Header("-------Melee Stats------")]
    [SerializeField] int meleeDmg;
    [SerializeField] float meleeDist;
    [SerializeField] int viewCone;

    [Header("-------Game Objects------")]
    [SerializeField] NavMeshAgent agent;
    public GameObject playertemp;
    [SerializeField] Renderer model;
    [SerializeField] GameObject FireBall;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Transform crotchPos;
    [SerializeField] Animator anim;
    [SerializeField] Animation RangeSpeed;

    Color origColor;

    int hpOrig;
    bool damageAnimCD;
    bool aggro;
    bool isShooting;
    bool playerinRange;
    bool doChargeAttack;
    float angleToPlayer;
    float stoppingDistOrig;
    int phase;
    int chargeCounter;
    Vector3 startingPos;
    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        hpOrig = hp;
        aggro = false;
        stoppingDistOrig = agent.stoppingDistance;
        doChargeAttack = false;
        phase = 2;
        chargeCounter = 0;
        origColor = model.material.color;
        HP_BOSS_UPDATE();
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.DrawRay(crotchPos.position, playerDir * meleeDist, Color.red);
        if (hp > 0)
        {
            if (doChargeAttack)
            {
                anim.SetBool("attack_long", false);
                chargeAttack();
            }
            if (playerinRange && canSeePlayer())
            {
                aggro = true;
            }
            if (aggro)
            {
                anim.SetTrigger("idle_combat");
            }

        }
    }
    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, headPos.position.y, playerDir.z), transform.forward);
        agent.SetDestination(gameManager.instance.player.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrig;
                if (!isShooting)
                {
                    anim.SetBool("attack_long", true);
                }

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
        gameManager.instance.bossHp.enabled = true;
        hp -= amount;
        HP_BOSS_UPDATE();
        StartCoroutine(changeColor());

        if (!damageAnimCD)
        {
            StartCoroutine(damageAnim());
        }
        if (hp > 0)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
        if (hp <= 200 && chargeCounter == 0)
        {
            doChargeAttack = true;
        }
        if (hp <= 100 && chargeCounter == 1)
        {
            doChargeAttack = true;
        }
        if (hp <= 0)
        {
            aggro = false;
            stoppingDistOrig = 100;
            anim.SetTrigger("dead");
            StartCoroutine(winTheGame());
        }
        
    }

    public void HP_BOSS_UPDATE()
    {
        gameManager.instance.bossHp.fillAmount = (float)hp / hpOrig;
    }
    IEnumerator changeColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        model.material.color = origColor;

    }

    IEnumerator damageAnim()
    {
        damageAnimCD = true;
        anim.SetTrigger("damage");
        yield return new WaitForSeconds(5);
        damageAnimCD = false;
    }

    //attack_long animation calling this function.
    IEnumerator shootThem()
    {
        isShooting = true;
        Instantiate(FireBall, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(0);
        isShooting = false;
    }

    public void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(crotchPos.position, transform.forward, out hit, meleeDist))
        {
            if (hit.collider.CompareTag("Player"))
            {
                iDamage dmg = hit.collider.GetComponent<iDamage>();
                if (hit.transform != transform && dmg != null)
                {
                    dmg.takeDamage(meleeDmg);
                }
            }
        }
    }

    public void chargeAttack()
    {
        isShooting = true;
        anim.SetBool("attack_long", false);
        anim.SetBool("move_forward_fast", true);
        stoppingDistOrig = 3.5f;
        agent.speed = 6;
        RaycastHit hit;
        if (Physics.Raycast(crotchPos.position, transform.forward, out hit, meleeDist))
        {
            if (hit.collider.CompareTag("Player"))
            {
                anim.SetTrigger("attack_short_001");
                doChargeAttack = false;
            }
        }

    }

    public void cancelChargeAttack()
    {
        stoppingDistOrig = 10;
        isShooting = false;
        agent.speed = 3.5f;
        anim.SetBool("move_forward_fast", false);
        phaseChange();
    }

    public void phaseChange()
    {
        chargeCounter++;
        anim.SetFloat("DoubleAttackSpeed", phase++);
    }

    IEnumerator winTheGame()
    {
        yield return new WaitForSeconds(6);
        gameManager.instance.bossNotKilled = false;
        gameManager.instance.updateGameGoal();
    }
}
