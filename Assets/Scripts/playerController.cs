using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;
using UnityEditor.Experimental.GraphView;

public class playerController : MonoBehaviour, IInteraction, IPickup
{
    public static playerController instance;

    [Header("Player")]
    [SerializeField] CharacterController player;
    public Camera playerCam;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] playerInfo playerInfo;

    Vector3 moveDir;

    enum shootingtype { shooting, none }
    [Header("Shooting")]
    public List<WeaponStats> weaponlist = new List<WeaponStats>();
    [SerializeField] shootingtype Shootingtype;
    public GameObject gunModel;
    [SerializeField] Transform shootPos;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    float shootTimer;
    public int spellListPos;
    public bool canShoot = true;


    /*
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

    int jumpCount;
    Vector3 playerVel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        playerInfo.origSpeed = playerInfo.Speed;
        //gunModel.GetComponent<MeshFilter>().sharedMesh = playerInfo.currentWeapon.GetComponent<MeshFilter>().sharedMesh;
        //gunModel.GetComponent<MeshRenderer>().sharedMaterial = playerInfo.currentWeapon.GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20, Color.black);

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

        selectSpell();
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
            playerInfo.Speed = playerInfo.origSpeed + playerInfo.sprintMod;
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

    void selectSpell()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && spellListPos < weaponlist.Count - 1)
        {
            spellListPos++;
            changeSpell();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && spellListPos > 0)
        {
            spellListPos--;
            changeSpell();
        }
        if (Input.anyKeyDown)
        {
            for (int i = 0; i <= 6; i++)
            {
                KeyCode key = KeyCode.Alpha0 + i;

                if (Input.GetKeyDown(key) && spellListPos < weaponlist.Count && weaponlist.Count != 1)
                {
                    int spellpos = i - 1;
                    spellListPos = spellpos;
                    changeSpell();
                }
            }
        }
        //listsTracker.spellListPos = spellListPos;
    }
    void changeSpell()
    {
        //Debug.Log("In change Spell");
        shootDamage = weaponlist[spellListPos].shootDMG;

        shootDist = weaponlist[spellListPos].shootDist;
        shootRate = weaponlist[spellListPos].shootRate;

        playerInfo.currentWeapon = weaponlist[spellListPos].model;

        //Debug.Log("Setting MeshFilter and MeshRenderer");
        gunModel.GetComponent<MeshFilter>().sharedMesh = weaponlist[spellListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = weaponlist[spellListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        //if (spellList[spellListPos] != null)
        //    listsTracker.spellList.Add(spellList[spellListPos]);
    }

    public void GetWeaponStats(WeaponStats weapon)
    {
        if (weapon.spellCheck == false)
        {
            //listsTracker.spellList.Add(weapon);
            //Debug.Log("Picked up a new Weapon: " + weapon.name);
            weaponlist.Add(weapon);
            spellListPos = weaponlist.Count - 1;

            changeSpell();
            weapon.spellCheck = true;

            //if (!Cheatmanager.instance.DescriptionCheat)
            //    gameManager.instance.DisplayDescription(this.weapon.spellManual);
        }

        //if (Cheatmanager.instance.spellCheat == true)
        //{
        //    spellList.Add(this.weapon);
        //    spellListPos = listsTracker.spellList.Count;

        //    changeSpell();
        //    this.weapon.spellCheck = false;
        //}
    }
}
