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
                yield return new WaitForSeconds(pauseTimer);
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
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facetargetSpeed);
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(flashRed());
        if (hp <= 0)
        {
            gameManager.instance.questItems.Add(this.tag);
            Destroy(gameObject);
            gameManager.instance.givePlayerXP(30);
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
