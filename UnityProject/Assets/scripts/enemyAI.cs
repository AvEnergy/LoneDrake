using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{


    [Header("--------AI STATS--------")]

    [SerializeField] int hp;
    [SerializeField] int speed;



    [Header("-------Game Objects------")]

    [SerializeField] NavMeshAgent agent;
    public GameObject playertemp;






    // Start is called before the first frame update
    void Start()
    {
        playertemp = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playertemp.transform.position);
    }
}
