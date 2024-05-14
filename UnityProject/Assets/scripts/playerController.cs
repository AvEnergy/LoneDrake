using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, iDamage
{
    [SerializeField] CharacterController controller;

    [Header("-------Player Stats------")]
    [SerializeField] int playerHP;
    [SerializeField] float playerHeat;
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
    [SerializeField] GameObject FTBurn;
    [SerializeField] Transform preMovement;

    [Header("-------Audio-------")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] AudioClip[] audFlame;
    [SerializeField] AudioClip audFireball;
    [SerializeField] float jumpVol;
    [SerializeField] float flameVol;
    [SerializeField] float fireballVol;

    bool playingSteps;
    public bool hasKey;
    bool isShooting;
    bool isFlameThrower;
    int jumpedTimes;
    int HPOrig;

    Vector3 playerVel;
    Vector3 moveDir;

    float HeatPlayer;
    // Start is called before the first frame update
    void Start()
    {
        hasKey = false;
        flamethrower.SetActive(false);
        HPOrig = playerHP;
        HeatPlayer = playerHeat;
        updatePlayerUI();
        spawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            jumpedTimes = 0;
            playerVel = Vector3.zero;
        }
        Movement();
        if (Input.GetButton("Sprint"))
        {
            speed = 12;
        }
        else
        {
            speed = 6;
        }
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        //Lets the player shoot the fireball when Lclick. If using the flamethrower, player will not be able to shoot.
        if (Input.GetButtonDown("Shoot") && !isFlameThrower && !gameManager.instance.isPaused)
        {
            aud.PlayOneShot(audFireball, fireballVol);
            StartCoroutine(shootFireball());
        }
        //Turns the flamethrower animation on when player clicks Rclick.
        if (Input.GetButtonDown("Fire2") && HeatPlayer > 0)
        {
            flamethrower.SetActive(true);
            isFlameThrower = true;
        }
        //Raycasting for the flamethrower.
        if (Input.GetButton("Fire2") && !isShooting)
        {
            aud.PlayOneShot(audFlame[Random.Range(0, audFlame.Length)], flameVol);
            StartCoroutine(shootFlameThrower());
            if (HeatPlayer <= 0)
            {
                flamethrower.SetActive(false);
            }
        }
        //Turns the flamethrower animation off when player released Rclick.
        if (Input.GetButtonUp("Fire2"))
        {
            aud.Stop();
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
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], jumpVol);
            jumpedTimes++;
            playerVel.y = jumpSpeed;
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

    //Simple raycasting.
    //Using isShooting here so we can tune the flamethrower to do rapid damage in close range.
    IEnumerator shootFlameThrower()
    {
        HeatPlayer = HeatPlayer - 0.5f;
        updatePlayerUI();
        isShooting = true;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            iDamage dmg = hit.collider.GetComponent<iDamage>();
            if (dmg != null && hit.transform != transform)
            {
                dmg.takeDamage(shootDamage);
            }
            else
            {
                Instantiate(FTBurn, hit.point, Quaternion.identity);
            }

        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        playerHP -= amount;
        updatePlayerUI();
        StartCoroutine(playerWasHit());
        if (playerHP <= 0)
        {
            gameManager.instance.youLoser();
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)playerHP / HPOrig;
        gameManager.instance.fireBar.fillAmount = HeatPlayer / 100;
        gameManager.instance.XPBar.fillAmount = (float)gameManager.instance.XP / 100;
    }

    IEnumerator playerWasHit()
    {
        gameManager.instance.playerIsHit.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        gameManager.instance.playerIsHit.SetActive(false);
    }

    public void spawnPlayer()
    {
        playerHP = HPOrig;
        updatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void GetHeat(int heat)
    {
        HeatPlayer += heat;
        updatePlayerUI();
    }

    //Trying to figure out how get the enemy to prefire at the location that player is moving towards.

    //private void preAimTrans()
    //{
    //    playerVel = moveDir / Time.deltaTime;
    //    playerAccel = playerVel / Time.deltaTime;
    //    preMovement.position += playerAccel;
    //}
}