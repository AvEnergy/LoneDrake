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
    [SerializeField] int roamDist;
    [SerializeField] int pauseTimer;
    [SerializeField] int viewCone;
    [SerializeField] int animSpeedTrans;

    [Header("-------Game Objects------")]
    [SerializeField] NavMeshAgent agent;
    public GameObject playertemp;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] GameObject arrow;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Transform dropPos;
    [SerializeField] GameObject loot;


    [Header("--------Audio-------")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audWalk;
    [SerializeField] float walkVol;


    [Header("-----Animations----")]
    private string[] animList;
    private string[] animList_Goblin;



    Color myColor;

    bool isDead;
    bool isShooting;
    bool playerinRange;
    bool destinationChosen;
    bool playingSteps;
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
       animList = new string[] { "death", "death2", "death3" };
       animList_Goblin = new string[] { "death", "death2", "death3" , "death4"};

    }

    // Update is called once per frame
    void Update()
    {
        float animSpeed = agent.velocity.normalized.magnitude;
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
        if(agent.velocity.magnitude > 0.01f && !playingSteps)
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
                yield return new WaitForSeconds(Random.Range(2, 6));
                Vector3 randomPos = Random.insideUnitSphere * roamDist;
                randomPos += startingPos;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
                agent.SetDestination(hit.position);
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

                if (!isShooting && CanShootAttack)
                {
                    anim.SetTrigger("shoot");
                }
                if (CanMeleeAttack)
                {
                    if (Physics.Raycast(shootPos.position, transform.forward, out hit, meleeDist))
                    {
                        anim.SetTrigger("attack");
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
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, facetargetSpeed * Time.deltaTime);
    }

    public void takeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }
        hp -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(flashRed());
        if (hp <= 0)
        {
            isDead = true;
            gameManager.instance.questItems.Add(this.tag);
            PlayDeath();
            gameManager.instance.givePlayerXP(30);

            if (Random.value >= 0.5f)
            {
                Instantiate(loot, dropPos.position, transform.rotation);
            }
        }
    }


    public void PlayDeath()
    {
        anim.ResetTrigger("attack");
        agent.isStopped = true;
        if (gameObject.name.Contains("Goblin")) 
        {
            anim.SetTrigger(animList_Goblin[Random.Range(0, animList_Goblin.Length)]);
        }
        else
        {
            anim.SetTrigger(animList[Random.Range(0, animList.Length)]);
        }
        Destroy(gameObject, 3.45f);
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

    //Function being called by animation.
    public void Attack()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(shootPos.position, transform.forward, out hit, meleeDist))
        {
            if (hit.collider.CompareTag("Player"))
            {
                IgnoreDamage dmg = hit.collider.GetComponent<IgnoreDamage>();
                if (hit.transform != transform && dmg != null)
                {
                    
                    dmg.IgnoreDamage(meleeDmg);
                    if (gameManager.instance.playerScript.playerHP <= 0)
                    {
                        gameManager.instance.killedby.text = "Enemy Melee";
                    }
                }
            }
        }
        
    }
}
