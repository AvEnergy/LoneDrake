using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, iDamage
{


    [Header("--------AI STATS--------")]

    [SerializeField] int hp;
    [SerializeField] int speed;
    [SerializeField] int shootRate;



    [Header("-------Game Objects------")]

    [SerializeField] NavMeshAgent agent;
    public GameObject playertemp;
    [SerializeField] Renderer model;
    [SerializeField] GameObject FireBall;
    [SerializeField] Transform shootPos;




    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        playertemp = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playertemp.transform.position);
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (!isShooting)
        {
            StartCoroutine(shootThem());
        }
    }
    public void takeDamage(int amount)
    {
        hp -= amount;

        StartCoroutine(flashRed());
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    IEnumerator shootThem()
    {
        isShooting = true;

        Instantiate(FireBall, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}
