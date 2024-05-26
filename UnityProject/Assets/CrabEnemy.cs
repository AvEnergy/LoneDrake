using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabEnemy : MonoBehaviour, iDamage
{
    [SerializeField] int hp;
    [SerializeField] int meleeDist;
    [SerializeField] int meleeDmg;

    [SerializeField] Renderer model;
    Color myColor;

    [SerializeField] Transform headPos;
    [SerializeField] int viewCone;
    [SerializeField] int facetargetSpeed;

    [SerializeField] int roamDist;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int animSpeedTrans;
    [SerializeField] Animator anim;
    [SerializeField] Collider collide;

    Vector3 startingPos;
    Vector3 playerDir;

    float stoppingDistOrig;
    bool destinationChosen;
    bool playerInRange;
    bool onCD;

    float angleToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        myColor = model.material.color;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(headPos.position, transform.forward, Color.red);
        float animSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Blend", Mathf.Lerp(anim.GetFloat("Blend"), animSpeed, Time.deltaTime * animSpeedTrans));
        if (playerInRange && !canSeePlayer())
        {
            StartCoroutine(roam());
        }
        else if (!playerInRange)
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
            yield return new WaitForSeconds(Random.Range(2, 6));
            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
            destinationChosen = false;
        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(flashRed());
        if (hp <= 0)
        {
            agent.isStopped = true;
            collide.enabled = false;
            gameManager.instance.questItems.Add(this.tag);
            gameManager.instance.givePlayerXP(30);
            anim.SetTrigger("death");
            Destroy(gameObject, 3.5f);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = myColor;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, playerDir.y, playerDir.z), transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            Debug.Log(hit.transform.name);
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(gameManager.instance.player.transform.position);
                agent.speed = 8;
                if (Physics.Raycast(headPos.position, transform.forward, out hit, meleeDist))
                {
                    if(!onCD)
                    {
                        StartCoroutine(attackCD());
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
        agent.speed = 4;
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facetargetSpeed);
    }

    public void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, transform.forward, out hit, meleeDist))
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

    IEnumerator attackCD()
    {
        onCD = true;
        yield return new WaitForSeconds(1.5f);
        anim.SetTrigger("attack");
        onCD = false;
    }
}
