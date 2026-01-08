using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController player;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] playerInfo playerInfo;

    Vector3 moveDir;

    enum shootingtype { shooting, none }
    [SerializeField] shootingtype Shootingtype;
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    //[SerializeField] GameObject playerObj;

    public Camera playerCam;

    [SerializeField] float shootRate;
    float shootTimer;

    int jumpCount;
    Vector3 playerVel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInfo.origSpeed = playerInfo.Speed;
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
