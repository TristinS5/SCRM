using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class playerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] CharacterController player;
    public Camera playerCam;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] playerInfo playerInfo;

    Vector3 moveDir;

    enum shootingtype { shooting, none }
    [Header("Shooting")]
    [SerializeField] shootingtype Shootingtype;
    public GameObject gunModel;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;

    /*
    enum shootchoice { shootraycast, spellList, teleportraycast }
    [SerializeField] shootchoice choice;
    public List<spellStats> spellList = new List<spellStats>();
    [SerializeField] GameObject spellModel;
    [SerializeField] GameObject spell;
    [SerializeField] Transform shootPos;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    float shootTimer;
    public int spellListPos;
    public bool canShoot = true;
    */


    [SerializeField] float shootRate;
    float shootTimer;

    int jumpCount;
    Vector3 playerVel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInfo.origSpeed = playerInfo.Speed;
        //gunModel.GetComponent<MeshFilter>().sharedMesh = playerInfo.currentWeapon.GetComponent<MeshFilter>().sharedMesh;
        //gunModel.GetComponent<MeshRenderer>().sharedMaterial = playerInfo.currentWeapon.GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        if (player.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        if (Input.GetButton("Fire1"))
        {
            if (Shootingtype == shootingtype.shooting && shootTimer >= shootRate)
                shoot();
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        player.Move(moveDir * playerInfo.Speed * Time.deltaTime);

        jump();

        player.Move(playerVel * Time.deltaTime);
        playerVel.y -= playerInfo.Gravity * Time.deltaTime;
    }

    void jump()
    {
        if (Input.GetButton("Jump") && jumpCount < playerInfo.jumpMax)
        {
            jumpCount++;
            playerVel.y = playerInfo.jumpForce;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            playerInfo.Speed = playerInfo.origSpeed * playerInfo.sprintMod;
        }
        if (Input.GetButtonUp("Sprint"))
        {
            playerInfo.Speed = playerInfo.origSpeed;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            //Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
    }

    void wallclimbing() { }

}
