using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPoint : MonoBehaviour
{
    [SerializeField] Renderer model;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            gameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(checkpoint());
        }
    }
    
    IEnumerator checkpoint()
    {
        gameManager.instance.menuCheckPoint.SetActive(true);
        yield return new WaitForSeconds(1);
        gameManager.instance.menuCheckPoint.SetActive(false);
    }

}
