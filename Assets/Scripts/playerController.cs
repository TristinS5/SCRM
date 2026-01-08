using System.Collections;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController player;
    [SerializeField] playerInfo playerInfo;

    Vector3 moveDir;

    enum shootingtype { shooting, none }
    [SerializeField] shootingtype Shootingtype;
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
        //player.transform.Rotate(0, 0, 45f);
    }

    void wallclimbing() { }

}
