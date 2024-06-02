using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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
    private string[] troll_attacks;


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

    [Header("-------Boss Stages-------")]
    
    [SerializeField] bool Stage2;
    [SerializeField] bool Stage3;

    [Header("-------Game Objects------")]
    [SerializeField] NavMeshAgent agent;
    public GameObject playertemp;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] GameObject arrow;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Transform dropPos;
    [SerializeField] GameObject MINIBOSS_HP_DISPLAY;
    [SerializeField] GameObject loot;


    [Header("--------Audio-------")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audWalk;
    [SerializeField] float walkVol;




    [Header("-------HP BAR TRACKING")]
    public Image miniBossHpBar;

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
    private bool stage2Complete = false;
    bool isDead;
    public bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = hp;
        currentAttackCooldown = attackCooldown;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        troll_attacks = new string[] {"attack", "attack2"}; 
        BossHPupdate();

        Debug.Log("Max Health: " + maxHealth + "HP: " + hp);
       // myColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(headPos.position, headPos.forward * 10, Color.yellow);
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

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;
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
                CanMeleeAttack = true;
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (!isShooting && CanShootAttack && currentAttackCooldown <= 0)
                {
                    anim.SetTrigger("shoot");
                    currentAttackCooldown = stage1Complete ? boostedAttackCooldown : attackCooldown;
                }
                if (CanMeleeAttack && agent.remainingDistance <= meleeDist)
                {
                        isAttacking = true;
                        anim.SetTrigger(troll_attacks[Random.Range(0, troll_attacks.Length)]);
                    isAttacking = false;
                       currentAttackCooldown = stage1Complete ? boostedAttackCooldown : attackCooldown;
                }
                if (Stage3 == true)
                {
                    anim.SetTrigger("attack2");
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
            MINIBOSS_HP_DISPLAY.SetActive(true);
        }
    }


    //checks if player is outside the collider
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerinRange = false;
            MINIBOSS_HP_DISPLAY.SetActive(false);
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

        if (isDead)
        {
            return;
        }
        hp -= amount;
        anim.SetTrigger("take_damage");
        BossHPupdate();
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (Stage2 == true)
        {
            if (!stage1Complete && hp <= maxHealth * .5f)
            {
                Debug.Log("Boss health below 50%!");
                anim.SetTrigger("run");
                speed += 4;
                attackCooldown /= 2;
                meleeDmg += 5;
                stage1Complete = true;
                currentHealth = hp;
                animSpeedTrans += 4;
                amount /= 2;
            }
        }
        if (Stage3 == true)
        {
            if (!stage2Complete && hp <= maxHealth * .25f)
            {
                Debug.Log("Boss health below 25%!");
                speed += 8;
                attackCooldown /= 4;
                meleeDmg += 10;
                stage2Complete = true;
                currentHealth = hp;
            }
        }
        if (hp <= 0)
        {
            isDead = true;
            BossDies();
            Instantiate(loot, dropPos.position, transform.rotation);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = myColor;
    }

    public void shootThem()
    {
        Instantiate(arrow, shootPos.position, transform.rotation);
    }

    public void BossHPupdate()
    {
        miniBossHpBar.fillAmount = (float)hp / maxHealth;
    }
    public void BossDies()
    {
        agent.isStopped = true;
        anim.ResetTrigger("attack");
        anim.SetTrigger("Dead");
        Destroy(gameObject, 3f);
        gameManager.instance.givePlayerXP(30);
    }
    //Function being called by animation.
    public void Attack()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position, transform.forward, Color.red);
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