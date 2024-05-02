using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, iDamage
{


    [Header("--------AI STATS--------")]

    [SerializeField] int hp;
    [SerializeField] int speed;
    [SerializeField] int facetargetSpeed;
    [SerializeField] bool iKnowWherePlayerIs;

    [Header("--------Shooting STATS--------")]
    [SerializeField] bool CanShootAttack;
    [SerializeField] float shootRate;

    [Header("-------Melee Stats------")]
    [SerializeField] bool CanMeleeAttack;
    [SerializeField] int meleeDmg;
    [SerializeField] float meleeDist;
    [SerializeField] float cooldownTime;
    [SerializeField] int roamDist;
    [SerializeField] int pauseTimer;
    [SerializeField] int viewCone;
    [SerializeField] int animSpeedTrans;

    [Header("-------Game Objects------")]
    [SerializeField] NavMeshAgent agent;
    public GameObject playertemp;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] GameObject FireBall;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    Color myColor;

    bool isShooting;
    bool meleeAttack;
    bool playerinRange;
    bool destinationChosen;
    float angleToPlayer;
    float stoppingDistOrig;
    Vector3 startingPos;
    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        stoppingDistOrig = agent.stoppingDistance;
       startingPos = transform.position;
        myColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        float animSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Blend", Mathf.Lerp(anim.GetFloat("Blend"), animSpeed, Time.deltaTime * animSpeedTrans));

        if (playerinRange && !canSeePlayer())
        {
            StartCoroutine(roam());
        }
        else if (!playerinRange)
        {
            StartCoroutine(roam());
        }

    }
    IEnumerator roam()
        {
            if (!destinationChosen && agent.remainingDistance < 0.05f)
            {
                destinationChosen = true;
                agent.stoppingDistance = 0;
                yield return new WaitForSeconds(pauseTimer);
                Vector3 randomPos = Random.insideUnitSphere * roamDist;
                randomPos += startingPos;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
                agent.SetDestination(hit.position);
                destinationChosen = false;
            }
        }
    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, headPos.position.y, playerDir.z), transform.forward);
        Debug.Log(angleToPlayer);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            Debug.Log(hit.transform.name);
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrig;
        agent.SetDestination(gameManager.instance.player.transform.position);
                if (!isShooting)
                    //StartCoroutine(shoot());              "shoot does not exist in this context"

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
        if (iKnowWherePlayerIs)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
        if (playerinRange)
        {
            playerDir = gameManager.instance.player.transform.position - transform.position;
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (!isShooting && CanShootAttack)
            {
                StartCoroutine(shootThem());
            }
            if(!meleeAttack && CanMeleeAttack)
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
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facetargetSpeed);
    }
    public void takeDamage(int amount)
    {
        hp -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(flashRed());
        if (hp <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            anim.SetTrigger("Death");
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = myColor;
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



    //Function to deal melee damage, still need to figure out a way to assign it to specific enemy
    public void Attack()
    {
        anim.SetTrigger("Attack");
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
