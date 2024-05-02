using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] float spawnTimer;
    [SerializeField] Transform[] spawnPos;

    int location;
    int spawnCount;
    bool isSpawning;
    bool startSpawning;
    // Start is called before the first frame update
    void Start()
    {
        location = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && !isSpawning && spawnCount < numToSpawn)
        {
            StartCoroutine(spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;
        location++;
        Instantiate(objectToSpawn, spawnPos[location].position, spawnPos[location].rotation);
        
        spawnCount++;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;
        location = 0;
    }
}
