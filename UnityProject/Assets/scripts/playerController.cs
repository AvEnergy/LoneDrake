using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] int playerHP;
    [SerializeField] int speed;
    [SerializeField] int jumpSpeed;
    [SerializeField] int maxJumps;
    [SerializeField] int gravity;

    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject fireball;
    bool isShooting;
    int jumpedTimes;

    Vector3 playerVel;
    Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetButtonDown("Shoot"))
        {
            StartCoroutine(shootFireball());
        }
    }

    void Movement()
    {
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpedTimes < maxJumps)
        {
            jumpedTimes++;
            playerVel.y = jumpSpeed;
        }

        if (controller.isGrounded)
        {
            jumpedTimes = 0;
            playerVel = Vector3.zero;
        }

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

    IEnumerator shootFireball()
    {
        isShooting = true;
        Instantiate(fireball, shootPos.position ,Camera.main.transform.rotation);
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }
}