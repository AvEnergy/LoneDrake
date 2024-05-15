using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GeneralBossAI : MonoBehaviour, iDamage
{


    [Header("--------AI STATS--------")]

    [SerializeField] int hp;
    [SerializeField] int speed;
    [SerializeField] int facetargetSpeed;
    [SerializeField] bool iKnowWherePlayerIs;
    [SerializeField] float boostedMoveSpeed;
    [SerializeField] float attackCooldown;
    [SerializeField] float boostedAttackCooldown;
    [SerializeField] private float minDistanceBetweenDestinations = 1.0f;


    [Header("--------Shooting STATS--------")]
    [SerializeField] bool CanShootAttack;
    [SerializeField] float shootRate;

    [Header("-------Melee Stats------")]
    [SerializeField] bool CanMeleeAttack;
    [SerializeField] int meleeDmg;
    [SerializeField] float meleeDist;
    [SerializeField] int roamDist;
    [SerializeField] int pauseTimer;
    [SerializeField] int viewCone;
    [SerializeField] int animSpeedTrans;
    [SerializeField] int boostedAnimSpeedTrans;

    [Header("-------Game Objects------")]
    [SerializeField] NavMeshAgent agent;
    public GameObject playertemp;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] GameObject arrow;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;


    [Header("--------Audio-------")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audWalk;
    [SerializeField] float walkVol;


    Color myColor;

    bool isShooting;
    bool playerinRange;
    bool destinationChosen;
    bool playingSteps;
    float angleToPlayer;
    float stoppingDistOrig;
    float moveSpeed;
    int maxHealth;
    Vector3 startingPos;
    Vector3 playerDir;

    private int currentHealth;
    private float currentAttackCooldown;
    private bool stage1Complete = false;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = hp;
        currentAttackCooldown = attackCooldown;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
       // myColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        float animSpeed = agent.velocity.normalized.magnitude;
        float moveSpeedValue = stage1Complete ? boostedMoveSpeed : speed;
        agent.speed = moveSpeedValue;
        anim.SetFloat("Blend", Mathf.Lerp(anim.GetFloat("Blend"), animSpeed, Time.deltaTime * animSpeedTrans));
        if (iKnowWherePlayerIs)
        {
            agent.stoppingDistance = stoppingDistOrig;
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
        if (playerinRange && !canSeePlayer())
        {
            StartCoroutine(roam());
        }
        else if (!playerinRange)
        {
            StartCoroutine(roam());
        }
        if (agent.velocity.magnitude > 0.01f && !playingSteps)
        {
            StartCoroutine(playfootSteps());
        }
       
    }
    IEnumerator roam()
    {
        if (!destinationChosen && agent.remainingDistance < 0.05f)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(pauseTimer);

            Vector3 randomPos = startingPos + Random.insideUnitSphere * roamDist;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPos, out hit, roamDist, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            destinationChosen = false;
        }
    }

    IEnumerator playfootSteps()
    {
        playingSteps = true;
        aud.PlayOneShot(audWalk, walkVol);
        yield return new WaitForSeconds(0.5f);
        playingSteps = false;
    }
    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, playerDir.y, playerDir.z), transform.forward);
        Debug.Log(angleToPlayer);
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            Debug.Log(hit.transform.name);
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (!isShooting && CanShootAttack && currentAttackCooldown <= 0)
                {
                    anim.SetTrigger("shoot");
                    currentAttackCooldown = stage1Complete ? boostedAttackCooldown : attackCooldown;
                }
                if (CanMeleeAttack && currentAttackCooldown <= 0)
                {
                    if (Physics.Raycast(shootPos.position, transform.forward, out hit, meleeDist))
                    {
                        anim.SetTrigger("attack");
                        currentAttackCooldown = stage1Complete ? boostedAttackCooldown : attackCooldown;
                    }
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
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facetargetSpeed);
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        //StartCoroutine(flashRed());

        if (!stage1Complete && hp <= maxHealth * .5f)
        {
            Debug.Log("Boss health below 50%!");
            speed += 4;
            attackCooldown /= 2;
            meleeDmg += 5;
            stage1Complete = true;
        }
        currentHealth = hp;
        if (hp <= 0)
        {
            anim.SetTrigger("Death");
            Destroy(gameObject);
            gameManager.instance.givePlayerXP(30);
        }
    }

    /*IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = myColor;
    }*/

    public void shootThem()
    {
        Instantiate(arrow, shootPos.position, transform.rotation);
    }

    //Function being called by animation.
    public void Attack()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(shootPos.position, transform.forward, out hit, meleeDist))
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
}