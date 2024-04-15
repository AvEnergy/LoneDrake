using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Header("-------Player Stats------")]
    [SerializeField] int playerHP;
    [SerializeField] int speed;
    [SerializeField] int jumpSpeed;
    [SerializeField] int maxJumps;
    [SerializeField] int gravity;

    [Header("-------Flamethrower Settings------")]
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;

    [Header("-------GameObjects------")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject fireball;
    [SerializeField] GameObject flamethrower;

    bool isShooting;
    bool isFlameThrower;
    int jumpedTimes;

    Vector3 playerVel;
    Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        flamethrower.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        //Lets the player shoot the fireball when Lclick. If using the flamethrower, player will not be able to shoot.
        if (Input.GetButtonDown("Shoot") && !isFlameThrower && !gameManager.instance.isPaused)
        {
            StartCoroutine(shootFireball());
        }
        //Turns the flamethrower animation on when player clicks Rclick.
        if (Input.GetButtonDown("Fire2"))
        {
            flamethrower.SetActive(true);
            isFlameThrower = true;
        }
        //Raycasting for the flamethrower.
        if (Input.GetButton("Fire2") && !isShooting)
        {
            StartCoroutine(shootFlameThrower());
        }
        //Turns the flamethrower animation off when player released Rclick.
        if (Input.GetButtonUp("Fire2"))
        {
            isFlameThrower = false;
            flamethrower.SetActive(false);
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

    //Creates and launches a fireball from shootPos. Not automatic, so player needs to click Lclick each time they want to shoot.
    IEnumerator shootFireball()
    {
        Instantiate(fireball, shootPos.position ,Camera.main.transform.rotation);
        yield return new WaitForSeconds(shootRate);
    }

    //Simple raycasting. (PROBLEM)
    //Using isShooting here so we can tune the flamethrower to do rapid damage in close range.
    IEnumerator shootFlameThrower()
    {
        isShooting = true;
        RaycastHit hit;
        //This if check is returning false for some reason and is skipping doing damage to target.
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            iDamage dmg = hit.collider.GetComponent<iDamage>();
            if (dmg != null && hit.transform != transform)
            {
                dmg.takeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}