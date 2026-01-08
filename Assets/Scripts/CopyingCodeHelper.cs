using UnityEngine;

public class CopyingCodeHelper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

//All Helper code
//player Controller code
/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class playerController : MonoBehaviour, IDamage, IPickup, IInteraction
{
    public static playerController instance;

    [Header("Player")]
    public CharacterController controller;
    public Camera mainCam;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] int animTransSpeed;

    [SerializeField] ItemCount ingredents;
    [SerializeField] ListsTracker listsTracker;

    [Header("Health")]
    public int HP;
    int HPOrig;
    [SerializeField] float healingCooldown;
    public int healingnum;
    public int numofhealpotions;
    float healTimer;
    public bool canTakeDam = true;

    [Header("Mana")]
    [SerializeField] int Mana;
    int ManaOrig;
    [SerializeField] int manaCost;
    [SerializeField] int shieldManaCost;
    [SerializeField] float manaCoolDownRate;
    float manaCooldownTimer;
    [SerializeField] float manaRegenRate;
    float manaRegenTimer;
    public int numofmanapotions;

    [Header("Oxygen")]
    public int Oxygen;
    public int OxygenOrig;
    [SerializeField] Transform WaterPos;
    [SerializeField] LayerMask waterLayer;

    [Header("Movement")]
    public float speed;
    public float origSpeed;
    [SerializeField] int sprintMod;
    bool inMud = false;
    bool canSprint = true;
    public bool canMove = true;
    public bool canStunned = true;
    [SerializeField] int superSpeed;

    enum shootchoice { shootraycast, spellList, teleportraycast }
    [Header("Shooting")]
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

    [Header("Shield")]
    [SerializeField] GameObject shield;
    [SerializeField] GameObject shieldBubble;
    [SerializeField] float shieldRate;
    float shieldTimer;

    [SerializeField] bool isTeleportingRaycast;
    [SerializeField] float teleportRate;
    float TeleportTimer;
    [SerializeField] int teleportDist;
    [SerializeField] GameObject TeleportModel;
    [SerializeField] GameObject spellTeleport;

    [Header("Jump")]
    [SerializeField] int jumpMax;
    [SerializeField] int jumpForce;
    [SerializeField] int Gravity;
    int jumpCount;
    int origJump;
    Vector3 playerVel;

    [Header("Audio")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audStep;
    [Range(0, 1)][SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;

    int InventoryPos = 0;
    public bool IsInventory;
    public bool PauseGameInInventory;

    bool isSprinting;
    bool isPlayingStep;
    bool isShielding = false;
    Coroutine co;

    public float potionTimerUse;
    float potionTimer;
    int OverMax;

    [SerializeField] itemStats healthPotionStats;
    [SerializeField] itemStats manaPotionStats;
    [SerializeField] itemStats healthPotionPlusStats;
    [SerializeField] itemStats manaPotionPlusStats;

    bool test;

    Vector3 moveDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        HPOrig = HP;
        ManaOrig = Mana;
        OxygenOrig = Oxygen;
        origSpeed = speed;
        origJump = jumpForce;
        test = true;
        IsInventory = false;
        canTakeDam = true;
        OverMax = 0;

        if (listsTracker.spellList.Count != 0 && spellList.Count == 0)
        {
            for (int i = 0; i < listsTracker.spellList.Count; i++)
            {
                spellList.Add(listsTracker.spellList[i]);
                changeSpell();
            }
        }

        gameManager.instance.UpdatePlayerMaxHPMPOXCount(HP, Mana, Oxygen);
        updatePlayerUI();
        if (spellList.Count > 0)
            changeSpell();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        if (isTeleportingRaycast)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * teleportDist, Color.blue);
        }

        Movement();
        sprint();

        if (Cheatmanager.instance.SpeedCheat)
        {
            speed = origSpeed * superSpeed;
        }
    }

    void Movement()
    {
        //setAnimPara();
        shootTimer += Time.deltaTime;
        healTimer += Time.deltaTime;
        TeleportTimer += Time.deltaTime;
        potionTimer += Time.deltaTime;

        if (Mana != ManaOrig)
            manaCooldownTimer += Time.deltaTime;

        if (test)
        {
            Oxygen -= 5;
            gameManager.instance.UpdatePlayerOXCount(-5);
            updatePlayerUI();
            test = false;
        }

        if (controller.isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        if (canMove)
        {
            moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

            if (controller.enabled == true)
                controller.Move(moveDir * speed * Time.deltaTime);

            jump();

            if (controller.enabled == true)
                controller.Move(playerVel * Time.deltaTime);

            playerVel.y -= Gravity * Time.deltaTime;
        }


        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            if (choice == shootchoice.shootraycast)
                shoot();
            if (choice == shootchoice.teleportraycast)
                teleportbyclick();
            if (choice == shootchoice.spellList && spellList.Count > 0 && Mana >= manaCost)
                shootSpell(canShoot);
        }
        if (Input.GetButton("Fire2") && TeleportTimer >= teleportRate && spellTeleport != null && canShoot)
        {
            Teleport();
            TeleportTimer = 0;
        }
        if (Input.GetKey("f"))
        {
            PotionUsed();
        }
        if (manaCooldownTimer >= manaCoolDownRate && Mana < ManaOrig)
        {
            ManaRegen();
        }

        if (Input.GetButtonDown("Shield") && shield != null && gameManager.instance.Shield.sprite != null)
        {
            isShielding = !isShielding;
        }
        if (isShielding && Mana > 0)
        {
            shieldTimer += Time.deltaTime;
            Shield();
        }
        else
        {
            isShielding = false;
            shieldBubble.SetActive(isShielding);
            shieldTimer = 0;
        }

        if (Input.GetButtonDown("Inventory"))
        {
            IsInventory = !IsInventory;
            Inventory();
        }

        if (potionTimer > potionTimerUse)
        {
            if (Input.GetKeyDown("z"))
            {
                HealPotion();
            }

            if (Input.GetKeyDown("x"))
            {
                ManaPotion();
            }

            if (Input.GetKeyDown("c"))
            {
                HealPotionPlus();
            }

            if (Input.GetKeyDown("v"))
            {
                ManaPotionPlus();
            }
        }

        selectSpell();

        gameManager.instance.UpdateIngredientCount(ingredents.baconCount, ingredents.beewaxCount, ingredents.mushroomCount);
    }

    void setAnimPara()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTransSpeed));
    }

    void Shield()
    {
        shield.SetActive(isShielding);
        shieldBubble.SetActive(isShielding);
        if (shieldTimer >= shieldRate)
        {
            Mana -= shieldManaCost;
            gameManager.instance.UpdatePlayerMPCount(-shieldManaCost);
            updatePlayerUI();
            shieldTimer = 0;
        }
    }

    void Inventory()
    {
        if (IsInventory == false)
        {
            gameManager.instance.StateUnpause();
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpForce;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }

    void sprint()
    {
        if (!canSprint)
        {
            if (isSprinting)
            {
                speed = origSpeed;
                isSprinting = false;
            }
            return;
        }
        if (Input.GetButtonDown("Sprint") && !isSprinting)
        {
            //speed += sprintMod;
            speed = origSpeed * sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint") && isSprinting)
        {
            //speed -= sprintMod;
            speed = origSpeed;
            isSprinting = false;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponentInParent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDMG(shootDamage);
            }
        }
    }

    void shootSpell(bool _canShoot)
    {
        if (_canShoot)
        {
            shootTimer = 0;
            manaCooldownTimer = 0;

            Mana -= manaCost;
            gameManager.instance.UpdatePlayerMPCount(-manaCost);
            updatePlayerUI();
            if (spellList[spellListPos].name != "Spell7_Teleport Spell" && spellList[spellListPos].name != "Spell2_Super_FireBall")
            {
                Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                {
                    Vector3 targetPoint = hit.point;
                    Vector3 shootDirection = (targetPoint - shootPos.position).normalized;

                    Instantiate(spell, shootPos.position, Quaternion.LookRotation(shootDirection));
                    if (spellList[spellListPos].hitEffect != null)
                        Instantiate(spellList[spellListPos].hitEffect, shootPos.position, Quaternion.LookRotation(shootDirection));
                }
            }
            else if (spellList[spellListPos].name == "Spell2_Super_FireBall")
            {
                Instantiate(spell, shootPos.position, Quaternion.LookRotation(Camera.main.transform.forward));
                if (spellList[spellListPos].hitEffect != null)
                    Instantiate(spellList[spellListPos].hitEffect, shootPos.position, Quaternion.LookRotation(Camera.main.transform.forward));
            }
        }
    }

    void PotionUsed()
    {
        if (craftingSystem.instance.IsHPPotion() && HP < HPOrig && healTimer > healingCooldown)
        {
            HealPotion();
        }
        else if (craftingSystem.instance.IsMPPotion())
        {
            ManaPotion();
        }
    }

    void HealPotion()
    {
        if (ingredents.HealthPotion > 0)
        {
            OverMax = HP + healthPotionStats.healFactor;
            if (OverMax > HPOrig)
            {
                OverMax = HPOrig - HP;
                gameManager.instance.UpdatePlayerHPCount(OverMax);
                HP = HPOrig;
                OverMax = 0;
            }
            else
            {
                gameManager.instance.UpdatePlayerHPCount(healthPotionStats.healFactor);
                HP += healthPotionStats.healFactor;
            }
            healTimer = 0;

            updatePlayerUI();
            ingredents.HealthPotion--;
            gameManager.instance.UpdatePotionCount();
        }
    }

    void ManaPotion()
    {
        if (ingredents.ManaPotion > 0)
        {
            OverMax = Mana + manaPotionStats.ManaFactor;
            if (OverMax > ManaOrig)
            {
                OverMax = ManaOrig - Mana;
                gameManager.instance.UpdatePlayerMPCount(OverMax);
                Mana = ManaOrig;
                OverMax = 0;
            }
            else
            {
                gameManager.instance.UpdatePlayerMPCount(manaPotionStats.ManaFactor);
                Mana += manaPotionStats.ManaFactor;
            }
            healTimer = 0;

            updatePlayerUI();
            ingredents.ManaPotion--;
            gameManager.instance.UpdatePotionCount();
        }
    }

    void HealPotionPlus()
    {
        if (ingredents.HealPlusPotion > 0)
        {
            OverMax = HP + healthPotionPlusStats.healFactor;
            if (OverMax > HPOrig)
            {
                OverMax = HPOrig - HP;
                gameManager.instance.UpdatePlayerHPCount(OverMax);
                HP = HPOrig;
                OverMax = 0;
            }
            else
            {
                gameManager.instance.UpdatePlayerHPCount(healthPotionPlusStats.healFactor);
                HP += healthPotionPlusStats.healFactor;
            }
            healTimer = 0;

            updatePlayerUI();
            ingredents.HealPlusPotion--;
            gameManager.instance.UpdatePotionCount();
        }
    }

    void ManaPotionPlus()
    {
        if (ingredents.ManaPlusPotion > 0)
        {
            OverMax = Mana + manaPotionPlusStats.ManaFactor;
            if (OverMax > ManaOrig)
            {
                OverMax = ManaOrig - Mana;
                gameManager.instance.UpdatePlayerMPCount(OverMax);
                Mana = ManaOrig;
                OverMax = 0;
            }
            else
            {
                gameManager.instance.UpdatePlayerMPCount(manaPotionPlusStats.ManaFactor);
                Mana += manaPotionPlusStats.ManaFactor;
            }
            healTimer = 0;

            updatePlayerUI();
            ingredents.ManaPlusPotion--;
            gameManager.instance.UpdatePotionCount();
        }
    }

    void ManaRegen()
    {
        manaRegenTimer += Time.deltaTime;
        if (manaRegenTimer >= manaRegenRate)
        {
            Mana += 1;
            gameManager.instance.UpdatePlayerMPCount(1);
            updatePlayerUI();
            manaRegenTimer = 0;
        }
        if (Mana == ManaOrig || Input.GetButton("Fire1") || shieldTimer > 0)
        {
            manaCooldownTimer = 0;
        }
    }

    void teleportbyclick()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Vector3 teleportPosition = hit.point;
            if (Vector3.Distance(transform.position, teleportPosition) <= teleportDist)
            {
                controller.enabled = false;
                teleportPosition.y = 1.0f;
                transform.position = teleportPosition;
                controller.enabled = true;
            }
        }
    }

    public void TakeDMG(int amount)
    {
        if (canTakeDam)
        {
            if (!Cheatmanager.instance.IsInvulnerable())
            {
                aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
                HP -= amount;
                gameManager.instance.UpdatePlayerHPCount(-amount);
                updatePlayerUI();
                if (canStunned)
                    StartCoroutine(Stunned(0.5f));
            }
        }
        else
        {

        }


        StartCoroutine(flashDamageScreen());
        //StartCoroutine(PostInvulnerable());


        if (HP <= 0)
        {
            //anim.SetTrigger("HP");
            gameManager.instance.YouLose();
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.playerManaBar.fillAmount = (float)Mana / ManaOrig;
        gameManager.instance.playerOxygenBarFiller.fillAmount = (float)Oxygen / OxygenOrig;
    }

    void selectSpell()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && spellListPos < spellList.Count - 1)
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

                if (Input.GetKeyDown(key) && spellListPos < spellList.Count && spellList.Count != 1)
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
        shootDamage = spellList[spellListPos].shootDMG;

        shootDist = spellList[spellListPos].shootDist;
        shootRate = spellList[spellListPos].shootRate;
        manaCost = spellList[spellListPos].manaCost;

        spellModel.GetComponent<MeshFilter>().sharedMesh = spellList[spellListPos].model.GetComponent<MeshFilter>().sharedMesh;
        spellModel.GetComponent<MeshRenderer>().sharedMaterial = spellList[spellListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        //if (spellList[spellListPos] != null)
        //    listsTracker.spellList.Add(spellList[spellListPos]);

        if (DisplayHotBar.instance == null)
        {

        }
        else
        {
            DisplayHotBar.instance.HotBar(spellListPos);
        }

        spell = spellList[spellListPos].spellProjectile;
    }

    public void GetSpellStats(spellStats spell)
    {
        if (spell.spellCheck)
        {
            if (spell.name != "Spell8_Shield" && spell.name != "Spell7_Teleport Spell")
            {
                listsTracker.spellList.Add(spell);
                spellList.Add(spell);
                spellListPos = spellList.Count - 1;

                changeSpell();
                spell.spellCheck = false;
            }
            else if (spell.name == "Spell8_Shield") //shield values
            {
                shield = spell.model;
                shieldManaCost = spell.manaCost;
                shieldRate = spell.shootRate;
                spell.spellCheck = false;

                gameManager.instance.Shield.sprite = spell.sprite;
                gameManager.instance.ShieldObj.SetActive(true);

            } // who watching?
            else
            {
                gameManager.instance.TeleportSlot.sprite = spell.sprite;
                teleportRate = spell.shootRate;

                TeleportModel.GetComponent<MeshFilter>().sharedMesh = spell.model.GetComponent<MeshFilter>().sharedMesh;
                TeleportModel.GetComponent<MeshRenderer>().sharedMaterial = spell.model.GetComponent<MeshRenderer>().sharedMaterial;

                spellTeleport = spell.spellProjectile;
                gameManager.instance.TeleportObj.SetActive(true);
            }
            if (!Cheatmanager.instance.DescriptionCheat)
                gameManager.instance.DisplayDescription(spell.spellManual);
        }

        if (Cheatmanager.instance.spellCheat == true)
        {
            spellList.Add(spell);
            spellListPos = listsTracker.spellList.Count;

            changeSpell();
            spell.spellCheck = false;
        }
    }

    bool SpellInventoryCheck()
    {
        return true;
    }

    public void GetItemStats(itemStats item)
    {
        //switch(item.itemName)
        if (item.itemName == "Bee Wax")
        {
            ingredents.beewaxCount++;
        }
        else if (item.itemName == "Boar Meat")
        {
            ingredents.baconCount++;
        }
        else if (item.itemName == "Mushroom")
        {
            ingredents.mushroomCount++;
        }
        else if (item.itemName == "Venom Gland")
        {
            ingredents.venomGlandCount++;
        }
        else if (item.itemName == "Health Potion")
        {
            ingredents.HealthPotion++;
        }
        else if (item.itemName == "Mana Potion")
        {
            ingredents.ManaPotion++;
        }
        else if (item.itemName == "Potion+")
        {
            ingredents.HealPlusPotion++;
        }
        else if (item.itemName == "ManaPotion+")
        {
            ingredents.ManaPlusPotion++;
        }

        if (item.bossCheck)
        {
            item.bossCheck = false;
            gameManager.instance.BossPickups();
        }

        if (item.firstTime && Cheatmanager.instance.DescriptionCheat == false && item.itemName != "Boss Egg" && item.itemName != "Cinnamon")
        {
            gameManager.instance.DisplayDescription(item.itemDescription);
            item.firstTime = false;
        }

        if (item.itemName != "Health Potion" && item.itemName != "Mana Potion" && item.itemName != "Potion+" && item.itemName != "ManaPotion+")
        {
            if (InventorySystem.instance.inventoryStats.Count <= gameManager.instance.items.Count && !InventorySystem.instance.inventoryStats.Contains(item))
            {
                InventorySystem.instance.inventoryStats.Add(item);
                item.Count++;
                InventorySystem.instance.StoredInventory(InventoryPos);
                InventoryPos++;
            }
            else
            {
                item.Count++;
            }
        }
        else
        {
            gameManager.instance.UpdatePotionCount();
        }

        InventorySystem.instance.VerifyCount();
    }

    void Teleport()
    {
        GameObject teleproj = Instantiate(spellTeleport, shootPos.position, Quaternion.LookRotation(Camera.main.transform.forward));
        teleproj.GetComponent<Teleport>().player = gameObject;
        teleproj.GetComponent<Teleport>().playercon = controller;
    }

    public void Stun(float duration)
    {
        StartCoroutine(Stunned(duration));
    }

    IEnumerator flashDamageScreen()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);
        if (isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        isPlayingStep = false;
    }

    IEnumerator Stunned(float duration)
    {
        canShoot = false;
        canMove = false;
        gameManager.instance.playerStunScreen.SetActive(true);
        yield return new WaitForSeconds(duration);
        gameManager.instance.playerStunScreen.SetActive(false);
        canMove = true;
        canShoot = true;
    }

    IEnumerator PostInvulnerable()
    {
        canTakeDam = false;
        yield return new WaitForSeconds(2f);
        canTakeDam = true;
    }

    public void EnterMud()
    {
        if (inMud) return;

        if (isSprinting)
        {
            speed = origSpeed;
            isSprinting = false;
        }


        origSpeed = Mathf.Max(1, speed / 2);
        speed = origSpeed;
        jumpForce = Mathf.Max(1, jumpForce / 2);

        canSprint = false;
        inMud = true;
    }

    public void ExitMud()
    {
        if (!inMud) return;

        origSpeed *= 2;
        speed = origSpeed;
        jumpForce = origJump;

        canSprint = true;
        inMud = false;
    }
}*/

//Displayed HotBar code
/*
using UnityEngine;
using UnityEngine.UI;

public class DisplayHotBar : MonoBehaviour
{
    public static DisplayHotBar instance;

    [Header("CurrentSpell")]
    [SerializeField] GameObject CurrentHotBar;
    public Image MainSpell;
    [SerializeField] GameObject OneSlots;

    [Header("TwoSlots")]
    [SerializeField] GameObject TwoSlots;
    public Image TwoSpellOne;
    public Image TwoSpellTwo;

    [Header("ThreeSlots")]
    [SerializeField] GameObject ThreeSlots;
    public Image ThreeSpellOne;
    public Image ThreeSpellTwo;
    public Image ThreeSpellThree;

    [Header("FourSlots")]
    [SerializeField] GameObject FourSlots;
    public Image FourSpellOne;
    public Image FourSpellTwo;
    public Image FourSpellThree;
    public Image FourSpellFour;

    [Header("FiveSlots")]
    [SerializeField] GameObject FiveSlots;
    public Image FiveSpellOne;
    public Image FiveSpellTwo;
    public Image FiveSpellThree;
    public Image FiveSpellFour;
    public Image FiveSpellFive;


    [SerializeField] ListsTracker listsTracker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(gameObject);
        //if (listsTracker.spellList != null)
        //    switch (listsTracker.spellList.Count)
        //    {
        //        case 1:
        //            if (MainSpell.sprite != null)
        //            {
        //                OneSlots.SetActive(true);
        //                MainSpell.sprite = listsTracker.spellList[0].sprite;
        //            }
        //            break;
        //        case 2:
        //            TwoSpellOne.sprite = listsTracker.spellList[0].sprite;
        //            TwoSpellTwo.sprite = listsTracker.spellList[1].sprite;
        //            OneSlots.SetActive(true);
        //            CurrentHotBar = TwoSlots;
        //            CurrentHotBar.SetActive(true);
        //            break;
        //        case 3:
        //            ThreeSpellOne.sprite = listsTracker.spellList[0].sprite;
        //            ThreeSpellTwo.sprite = listsTracker.spellList[1].sprite;
        //            ThreeSpellThree.sprite = listsTracker.spellList[2].sprite;
        //            OneSlots.SetActive(true);
        //            CurrentHotBar = ThreeSlots;
        //            CurrentHotBar.SetActive(true);
        //            break;
        //        case 4:
        //            FourSpellOne.sprite = listsTracker.spellList[0].sprite;
        //            FourSpellTwo.sprite = listsTracker.spellList[1].sprite;
        //            FourSpellThree.sprite = listsTracker.spellList[2].sprite;
        //            FourSpellFour.sprite = listsTracker.spellList[3].sprite;
        //            OneSlots.SetActive(true);
        //            CurrentHotBar = FourSlots;
        //            CurrentHotBar.SetActive(true);
        //            break;
        //        case 5:
        //            FiveSpellOne.sprite = listsTracker.spellList[0].sprite;
        //            FiveSpellTwo.sprite = listsTracker.spellList[1].sprite;
        //            FiveSpellThree.sprite = listsTracker.spellList[2].sprite;
        //            FiveSpellFour.sprite = listsTracker.spellList[3].sprite;
        //            FiveSpellFive.sprite = listsTracker.spellList[4].sprite;
        //            OneSlots.SetActive(true);
        //            CurrentHotBar = FiveSlots;
        //            CurrentHotBar.SetActive(true);
        //            break;
        //        default:
        //            break;
        //    }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HotBar(int spell)
    {
        if (spell < listsTracker.spellList.Count)
        {
            switch (listsTracker.spellList.Count - 1)
            {
                case 0:
                    if (OneSlots != null)
                        OneSlots.SetActive(true);
                    MainSpell.sprite = listsTracker.spellList[spell].sprite;
                    TwoSpellOne.sprite = listsTracker.spellList[spell].sprite;
                    ThreeSpellOne.sprite = listsTracker.spellList[spell].sprite;
                    FourSpellOne.sprite = listsTracker.spellList[spell].sprite;
                    FiveSpellOne.sprite = listsTracker.spellList[spell].sprite;
                    break;
                case 1:
                    if (CurrentHotBar != TwoSlots)
                    {
                        //CurrentHotBar.SetActive(false);
                        CurrentHotBar = TwoSlots;
                        CurrentHotBar.SetActive(true);
                    }
                    switch (spell)
                    {
                        case 0:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            TwoSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 1:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            TwoSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 2:
                            listsTracker.spellListPos -= 1;
                            break;
                    }
                    break;
                case 2:
                    if (CurrentHotBar == TwoSlots)
                    {
                        CurrentHotBar.SetActive(false);
                        CurrentHotBar = ThreeSlots;
                        CurrentHotBar.SetActive(true);
                    }
                    switch (spell)
                    {
                        case 0:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            TwoSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 1:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            TwoSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 2:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 3:
                            listsTracker.spellListPos -= 1;
                            break;
                    }
                    break;
                case 3:
                    if (CurrentHotBar == ThreeSlots)
                    {
                        CurrentHotBar.SetActive(false);
                        CurrentHotBar = FourSlots;
                        CurrentHotBar.SetActive(true);
                    }
                    switch (spell)
                    {
                        case 0:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            TwoSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 1:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            TwoSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 2:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 3:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellFour.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellFour.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 4:
                            listsTracker.spellListPos -= 1;
                            break;
                    }
                    break;
                case 4:
                    if (CurrentHotBar == FourSlots)
                    {
                        CurrentHotBar.SetActive(false);
                        CurrentHotBar = FiveSlots;
                        CurrentHotBar.SetActive(true);
                    }
                    switch (spell)
                    {
                        case 0:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            TwoSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellOne.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 1:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            TwoSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellTwo.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 2:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            ThreeSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellThree.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 3:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            FourSpellFour.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellFour.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 4:
                            MainSpell.sprite = listsTracker.spellList[spell].sprite;
                            FiveSpellFive.sprite = listsTracker.spellList[spell].sprite;
                            break;
                        case 5:
                            listsTracker.spellListPos -= 1;
                            break;

                    }
                    break;
            }
        }
    }
}
*/

//Button Fuctions code
/*
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        gameManager.instance.StateUnpause();
    }

    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            SceneManager.LoadScene("Forest1");
        }
        else if (SceneManager.GetActiveScene().name == "Forest1")
        {
            SceneManager.LoadScene("Showcase Level");
        }
        else if (SceneManager.GetActiveScene().name == "Showcase Level")
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gameManager.instance.StateUnpause();
    }

    public void Quit()
    {
        if (SceneManager.GetActiveScene().name == "MainScene" || SceneManager.GetActiveScene().name == "Forest1" || SceneManager.GetActiveScene().name == "Showcase Level")
        {
            SceneManager.LoadScene("LevelSelect");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();

#endif
        }
    }
}
*/

//Camera code
/*
using UnityEngine;

public class cameraController : MonoBehaviour
{
	[SerializeField] int sensitivity;
	[SerializeField] int lockVertMin, lockVertMax;
	[SerializeField] bool invertY;

	float rotX;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

		if (invertY)
			rotX += mouseY;
		else
			rotX -= mouseY;

		rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);
		transform.localRotation = Quaternion.Euler(rotX, 0, 0);
		transform.parent.Rotate(Vector3.up * mouseX);
	}
}

 */

//Cheat Manager code
/*
using System.Collections.Generic;
using UnityEngine;

public class Cheatmanager : MonoBehaviour
{

    public static Cheatmanager instance;

    bool invulnerable = false;

    public List<spellStats> ListAllSpells = new List<spellStats>();
    public bool spellCheat;

    public bool DescriptionCheat;
    public bool SpeedCheat;

    private KeyCode[] invulnerablecheatCode =
    {
        KeyCode.I,
        KeyCode.N,
        KeyCode.V,
        KeyCode.U,
        KeyCode.N
    };
    private KeyCode[] allSpellCheatCode =
    {
        KeyCode.M,
        KeyCode.A,
        KeyCode.G,
        KeyCode.I,
        KeyCode.C
    };
    private KeyCode[] DescriptionBoxCheatCode =
    {
        KeyCode.D,
        KeyCode.E,
        KeyCode.S,
        KeyCode.C,
        KeyCode.T,
    };
    private KeyCode[] SuperSpeedCheatCode =
    {
        KeyCode.S,
        KeyCode.U,
        KeyCode.P,
        KeyCode.E,
        KeyCode.R,
    };

    private int curIndex = 0;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(DescriptionBoxCheatCode[curIndex]))
            {
                curIndex++;
                if (curIndex >= DescriptionBoxCheatCode.Length)
                {
                    if (DescriptionCheat)
                    {
                        DescriptionCheat = false;
                    }
                    else
                    {
                        DescriptionCheat = true;
                    }
                    curIndex = 0;
                }
            }
            else if (Input.GetKeyDown(allSpellCheatCode[curIndex]))
            {
                curIndex++;
                if (curIndex >= allSpellCheatCode.Length)
                {
                    if (spellCheat)
                    {

                        gameManager.instance.MainSpell.sprite = null;
                        gameManager.instance.SpellOne.sprite = null;
                        gameManager.instance.SpellTwo.sprite = null;
                        gameManager.instance.SpellThree.sprite = null;
                        gameManager.instance.SpellFour.sprite = null;
                        gameManager.instance.SpellFive.sprite = null;

                        playerController.instance.spellList.Clear();
                        playerController.instance.spellListPos = 0;
                        spellCheat = false;
                    }
                    else
                    {
                        for (int index = 0; index < ListAllSpells.Count; index++)
                        {
                            spellCheat = true;
                            ListAllSpells[index].spellCheck = false;
                            playerController.instance.GetSpellStats(ListAllSpells[index]);
                        }
                    }
                    curIndex = 0;
                }
            }
            else if (Input.GetKeyDown(invulnerablecheatCode[curIndex]))
            {
                curIndex++;
                if (curIndex >= invulnerablecheatCode.Length)
                {
                    if (invulnerable)
                    {
                        invulnerable = false;
                        gameManager.instance.GodMode.SetActive(false);
                        gameManager.instance.NormalMode.SetActive(true);
                    }
                    else
                    {
                        invulnerable = true;
                        gameManager.instance.GodMode.SetActive(true);
                        gameManager.instance.NormalMode.SetActive(false);
                    }
                    curIndex = 0;
                }
            }
            else if (Input.GetKeyDown(SuperSpeedCheatCode[curIndex]))
            {
                curIndex++;
                if (curIndex >= SuperSpeedCheatCode.Length)
                {
                    if (SpeedCheat)
                    {
                        SpeedCheat = false;
                        playerController.instance.speed = playerController.instance.origSpeed;
                    }
                    else
                    {
                        SpeedCheat = true;
                    }
                    curIndex = 0;
                }
            }
            else if (Input.anyKeyDown)
            {
                curIndex = 0;
            }
        }
    }

    public bool IsInvulnerable()
    {
        return invulnerable;
    }
}
 */

//Game Manager code
/*
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] TMP_Text winText;
    [SerializeField] string winOmeletText;
    [SerializeField] string winPumpkinPieText;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text loseText;
    [SerializeField] string loseOmeletText;
    [SerializeField] string losePumpkinPieText;
    public GameObject Inventory;

    [Header("Texts")]
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text bossHPCountText;
    [SerializeField] TMP_Text bossHPMaxText;
    [SerializeField] TMP_Text baconCountText;
    [SerializeField] TMP_Text beesWaxCountText;
    [SerializeField] TMP_Text mushroomCountText;
    [SerializeField] TMP_Text baconGoalText;
    [SerializeField] TMP_Text beesWaxGoalText;
    [SerializeField] TMP_Text mushroomGoalText;

    [Header("Player Health")]
    [SerializeField] TMP_Text playerHPCountText;
    [SerializeField] TMP_Text playerHPMaxText;
    public Image playerHPBar;
    int playerHPCountOrig;
    int playerHPMaxOrig;

    [Header("Player Mana")]
    [SerializeField] TMP_Text playerMPCountText;
    [SerializeField] TMP_Text playerMPMaxText;
    public Image playerManaBar;
    int playerMPCountOrig;
    int playerMPMaxOrig;

    [Header("Player Oxgyen")]
    [SerializeField] TMP_Text playerOXCountText;
    [SerializeField] TMP_Text playerOXMaxText;
    public Image playerOxygenBarFiller;
    int playerOXCountOrig;
    int playerOXMaxOrig;

    [Header("Potions Text")]
    [SerializeField] TMP_Text healpotionText;
    int healpotionCountOrig;
    [SerializeField] TMP_Text manapotionText;
    int manapotionCountOrig;
    [SerializeField] TMP_Text healpotionplusText;
    int healpotionplusCountOrig;
    [SerializeField] TMP_Text manapotionplusText;
    int manapotionplusCountOrig;


    [Header("HotBar")]
    public Image MainSpell;
    public Image SpellOne;
    public Image SpellTwo;
    public Image SpellThree;
    public Image SpellFour;
    public Image SpellFive;
    [SerializeField] List<spellStats> Spell = new List<spellStats>();
    public List<itemStats> items = new List<itemStats>();
    [SerializeField] ItemCount ItemCount;
    [SerializeField] ListsTracker AllLists;

    public GameObject TeleportObj;
    public Image TeleportSlot;
    public GameObject ShieldObj;
    public Image Shield;

    [Header("Description")]
    public GameObject textBox;
    public TMP_Text textDescription;

    [Header("Dialogue")]
    public GameObject DialogueBox;
    public TMP_Text DialogueDescription;

    [Header("Screens")]
    public GameObject playerDamageScreen;
    public GameObject playerStunScreen;

    public GameObject GodMode;
    public GameObject NormalMode;

    public GameObject player;
    public playerController playerScript;
    public Image bossHPBar;
    public int baconGoalPI;
    public int beesWaxGoalPI;
    public int mushroomGoalPI;

    public string startupDialogue;
    public bool isPaused;

    float timeScaleOrig;
    int gameGoalCount;
    int bossHPCountOrig;
    int bossHPMaxOrig;
    int baconCount;
    int beesWaxCount;
    int mushroomCount;
    int baconGoal;
    int beesWaxGoal;
    int mushroomGoal;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
        }
        timeScaleOrig = Time.timeScale;

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            AllReset();
            DisplayDescription(startupDialogue);

            winText.text = winOmeletText;
            loseText.text = loseOmeletText;
        }
        if (SceneManager.GetActiveScene().name == "Forest1")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            winText.text = winPumpkinPieText;
            loseText.text = losePumpkinPieText;
        }
        if (SceneManager.GetActiveScene().name == "LevelSelect")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        UpdatePotionCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (textBox.activeSelf != true && DialogueBox.activeSelf != true)
            {
                if (menuActive == null)
                {
                    StatePause();
                    menuActive = menuPause;

                    if (menuActive != null)
                    {
                        menuActive.SetActive(true);
                    }

                }
                else if (menuActive == menuPause)
                    StateUnpause();
            }
        }

        if (playerController.instance != null && playerController.instance.IsInventory)
        {
            if (playerController.instance.IsInventory)
            {
                if (menuActive != menuPause)
                {
                    if (playerController.instance.PauseGameInInventory)
                        StatePause();

                    menuActive = Inventory;
                    menuActive.SetActive(true);
                }
                else
                {
                    playerController.instance.IsInventory = false;
                }
            }
        }


        if (Input.GetKey("q") && textBox.activeSelf == true)
        {
            HideDescription();
        }
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (playerController.instance != null)
            playerController.instance.canShoot = false;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        if (playerController.instance != null)
            playerController.instance.canShoot = true;
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void DisplayDescription(string description)
    {
        textBox.SetActive(true);
        textDescription.text = description;

        isPaused = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideDescription()
    {
        textBox.SetActive(false);
        textDescription.text = "";

        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void DisplayDialogue(string Dialogue)
    {
        DialogueBox.SetActive(true);
        DialogueDescription.text = Dialogue;

        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideDialogue()
    {
        DialogueBox.SetActive(false);
        DialogueDescription.text = "";

        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;
        //gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void UpdatePlayerHPCount(int amount)
    {
        playerHPCountOrig += amount;
        playerHPCountText.text = playerHPCountOrig.ToString("F0");
    }

    public void UpdatePlayerMPCount(int amount)
    {
        playerMPCountOrig += amount;
        playerMPCountText.text = playerMPCountOrig.ToString("F0");
    }

    public void UpdatePlayerOXCount(int amount)
    {
        playerOXCountOrig += amount;
        playerOXCountText.text = playerOXCountOrig.ToString("F0");
    }

    public void UpdatePlayerMaxHPMPOXCount(int hpAmount, int mpAmount, int oxAmount)
    {
        UpdatePlayerHPCount(hpAmount);
        UpdatePlayerMPCount(mpAmount);
        UpdatePlayerOXCount(oxAmount);

        playerHPMaxOrig += hpAmount;
        playerHPMaxText.text = playerHPMaxOrig.ToString("F0");

        playerMPMaxOrig += mpAmount;
        playerMPMaxText.text = playerMPMaxOrig.ToString("F0");

        playerOXMaxOrig += oxAmount;
        playerOXMaxText.text = playerOXMaxOrig.ToString("F0");
    }

    public void UpdateBossHPCount(int amount)
    {
        bossHPCountOrig += amount;
        bossHPCountText.text = bossHPCountOrig.ToString("F0");
    }

    public void UpdatePotionCount()
    {
        if (healpotionText != null)
        {
            healpotionCountOrig = ItemCount.HealthPotion;
            healpotionText.text = healpotionCountOrig.ToString("F0");
        }

        if (manapotionText != null)
        {
            manapotionCountOrig = ItemCount.ManaPotion;
            manapotionText.text = manapotionCountOrig.ToString("F0");
        }

        if (healpotionplusText != null)
        {
            healpotionplusCountOrig = ItemCount.HealPlusPotion;
            healpotionplusText.text = healpotionplusCountOrig.ToString("F0");
        }

        if (manapotionplusText != null)
        {
            manapotionplusCountOrig = ItemCount.ManaPlusPotion;
            manapotionplusText.text = manapotionplusCountOrig.ToString("F0");
        }
    }

    public void UpdateIngredientCount(int baconAmount, int beesWaxAmount, int mushroomAmount)
    {
        baconCount = baconAmount;
        beesWaxCount = beesWaxAmount;
        mushroomCount = mushroomAmount;

        baconCountText.text = baconCount.ToString("F0");
        beesWaxCountText.text = beesWaxCount.ToString("F0");
        mushroomCountText.text = mushroomCount.ToString("F0");
    }

    public void UpdateIngredientGoal(int baconAmount, int beesWaxAmount, int mushroomAmount)
    {
        baconGoal = baconAmount;
        beesWaxGoal = beesWaxAmount;
        mushroomGoal = mushroomAmount;

        if (baconGoalText != null)
        {
            baconGoalText.text = baconGoal.ToString("F0");
        }
        if (beesWaxGoalText != null)
        {
            beesWaxGoalText.text = beesWaxGoal.ToString("F0");
        }

        if (mushroomGoalText != null)
        {
            mushroomGoalText.text = mushroomGoal.ToString("F0");
        }
    }

    public void BossPickups()
    {
        YouWin();
    }

    public void YouWin()
    {
        StatePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    void PickUpCheck()
    {
        for (int i = 0; i < Spell.Count; i++)
            Spell[i].spellCheck = true;
        for (int i = 0; i < items.Count; i++)
            items[i].firstTime = true;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == "Boss Egg" || items[i].itemName == "Cinnamon")
                items[i].bossCheck = true;
        }
    }

    void InventoryReset()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Count = 0;
        }
    }

    void AllReset()
    {
        PickUpCheck();
        if (ItemCount != null)
        {
            ItemCount.beewaxCount = 0;
            ItemCount.baconCount = 0;
            ItemCount.mushroomCount = 0;
            ItemCount.venomGlandCount = 0;
            ItemCount.cinnamonCount = 0;

            ItemCount.HealthPotion = 0;
            ItemCount.ManaPotion = 0;
            ItemCount.HealPlusPotion = 0;
            ItemCount.ManaPlusPotion = 0;
        }
        InventoryReset();
        if (AllLists != null)
        {
            if (AllLists.spellList.Count > 0 && AllLists.spellList != null)
            {
                AllLists.spellList.Clear();
                AllLists.spellListPos = 0;
            }
            if (AllLists.ItemList.Count > 0 && AllLists.ItemList != null)
                AllLists.ItemList.Clear();
        }

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        menuActive = null; // clear any leftover UI state

    }
}
 */

//ItemCount code
/*
using UnityEngine;

[CreateAssetMenu]
public class ItemCount : ScriptableObject
{
    [Header("Ingredents Count")]
    public int beewaxCount;
    public int baconCount;
    public int mushroomCount;
    public int venomGlandCount;
    public int cinnamonCount;
    public int leafCount;

    [Header("Potions Count")]
    public int HealthPotion;
    public int ManaPotion;
    public int HealPlusPotion;
    public int ManaPlusPotion;
}
 */

//List Tracker code
/*
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class ListsTracker : ScriptableObject
{
    public List<spellStats> spellList = new List<spellStats>();
    public int spellListPos;
    public List<itemStats> ItemList = new List<itemStats>();
}
 */

//Inventory System code
/*
 using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;
    public List<itemStats> inventoryStats = new List<itemStats>();

    public ItemCount ingredents;

    public GameObject Inventory;

    public GameObject RowOne;
    public Image R1SlotOne;
    public Image R1SlotTwo;
    public Image R1SlotThree;
    public Image R1SlotFour;
    public Image R1SlotFive;

    public TMP_Text R1SlotOnetext;
    public TMP_Text R1SlotTwotext;
    public TMP_Text R1SlotThreetext;
    public TMP_Text R1SlotFourtext;
    public TMP_Text R1SlotFivetext;

    public GameObject RowTwo;
    public Image R2SlotOne;
    public Image R2SlotTwo;
    public Image R2SlotThree;
    public Image R2SlotFour;
    public Image R2SlotFive;

    public TMP_Text R2SlotOnetext;
    public TMP_Text R2SlotTwotext;
    public TMP_Text R2SlotThreetext;
    public TMP_Text R2SlotFourtext;
    public TMP_Text R2SlotFivetext;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // or log a warning

        RowOne.SetActive(false);
        RowTwo.SetActive(false);
        Inventory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateInventoryCount()
    {
        for (int index = 0; index < inventoryStats.Count; index++)
        {
            switch (index)
            {
                //Row1
                case 0:
                    R1SlotOnetext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                case 1:
                    R1SlotTwotext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                case 2:
                    R1SlotThreetext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                case 3:
                    R1SlotFourtext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                case 4:
                    R1SlotFivetext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                //Row2
                case 5:
                    RowTwo.SetActive(true);

                    R2SlotOnetext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                case 6:
                    R2SlotTwotext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                case 7:
                    R2SlotThreetext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                case 8:
                    R2SlotFourtext.text = inventoryStats[index].Count.ToString("F0");
                    break;
                case 9:
                    R2SlotFivetext.text = inventoryStats[index].Count.ToString("F0");
                    break;
            }
        }
    }

    public void StoredInventory(int item)
    {
        switch (item)
        {
            //Row1
            case 0:
                RowOne.SetActive(true);
                R1SlotOne.sprite = inventoryStats[item].sprite;
                break;
            case 1:
                R1SlotTwo.sprite = inventoryStats[item].sprite;
                break;
            case 2:
                R1SlotThree.sprite = inventoryStats[item].sprite;
                break;
            case 3:
                R1SlotFour.sprite = inventoryStats[item].sprite;
                break;
            case 4:
                R1SlotFive.sprite = inventoryStats[item].sprite;
                break;
            //Row2
            case 5:
                RowTwo.SetActive(true);

                R2SlotOne.sprite = inventoryStats[item].sprite;
                break;
            case 6:
                R2SlotTwo.sprite = inventoryStats[item].sprite;
                break;
            case 7:
                R2SlotThree.sprite = inventoryStats[item].sprite;
                break;
            case 8:
                R2SlotFour.sprite = inventoryStats[item].sprite;
                break;
            case 9:
                R2SlotFive.sprite = inventoryStats[item].sprite;
                break;

        }
    }

    public void VerifyCount()
    {
        for (int i = 0; i < inventoryStats.Count; i++)
        {
            if (inventoryStats[i].itemName == "Bee Wax")
            {
                inventoryStats[i].Count = ingredents.beewaxCount;
            }
            else if (inventoryStats[i].itemName == "Boar Meat")
            {
                inventoryStats[i].Count = ingredents.baconCount;
            }
            else if (inventoryStats[i].itemName == "Mushroom")
            {
                inventoryStats[i].Count = ingredents.mushroomCount;
            }
            else if (inventoryStats[i].itemName == "Venom Gland")
            {
                inventoryStats[i].Count = ingredents.venomGlandCount;
            }
            else if (inventoryStats[i].itemName == "Cinnamon")
            {
                inventoryStats[i].Count = ingredents.cinnamonCount;
            }
            UpdateInventoryCount();
        }
    }
}
*/

//Game Event Manager code
/*
 using UnityEngine;
using System.Collections.Generic;

public class gameEventManager : MonoBehaviour
{
    public static gameEventManager instance;

    [SerializeField] List<DialogueStats> dialogueStatsList = new List<DialogueStats>();
    [SerializeField] List<GameObject> FairyTriggerList = new List<GameObject>();

    int ListPos = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        if (dialogueStatsList != null && FairyTriggerList != null)
        {
            for (; ListPos < dialogueStatsList.Count;)
            {
                for (int index = 0; index < dialogueStatsList[ListPos].dialogue.Count; index++)
                {
                    // Main issue is forgeting to put triggers not the spawners
                    FairyTriggerList[ListPos].GetComponent<SelectionSpawner>().dialogue.Add(dialogueStatsList[ListPos].dialogue[index]);
                }
                ListPos++;
            }
            ListPos = 0;
        }
    }

    public void EventOff(GameObject Spawner)
    {
        Spawner.SetActive(false);
    }
}
*/

//Crafting System code
/*
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class craftingSystem : MonoBehaviour
{
    public static craftingSystem instance;

    [Header("Crafting Display")]
    [SerializeField] GameObject craftActive;

    [Header("Ingredents Display")]
    [SerializeField] Image ingredentOne;
    [SerializeField] Image ingredentTwo;
    [SerializeField] Image result;

    [Header("Recipes")]
    [SerializeField] List<Recipes> recipes = new List<Recipes>();
    [SerializeField] ItemCount ingredents;


    int recipePos;
    bool IsHealPotion;
    bool IsManaPotion;
    bool IsHealPotionPlus;
    [SerializeField] float PotionCooldown;
    float makePotionsTimer;

    void Awake()
    {
        instance = this;
        craftActive.SetActive(true);

        IsHealPotion = true;
        IsManaPotion = false;
        IsHealPotionPlus = false;

        if (recipes != null)
        {
            ingredentOne.GetComponent<Image>().sprite = recipes[recipePos].ingredentsOne;
            ingredentTwo.GetComponent<Image>().sprite = recipes[recipePos].ingredentsTwo;
            result.GetComponent<Image>().sprite = recipes[recipePos].result;
            recipePos = 1;
        }
    }

    void Update()
    {
        makePotionsTimer += Time.deltaTime;
        if (playerController.instance.IsInventory)
        {
            if (Input.GetKeyDown("r"))
            {
                if (recipePos < recipes.Count)
                {
                    SetCraft();
                    recipePos++;
                }
                if (recipePos >= recipes.Count)
                {
                    recipePos = 0;
                }
            }
            if (Input.GetKeyDown("c") && makePotionsTimer > PotionCooldown)
            {
                CraftPotion();
            }
        }
    }

    void SetCraft()
    {
        if (recipes[recipePos] != null)
        {
            ingredentOne.GetComponent<Image>().sprite = recipes[recipePos].ingredentsOne;
            ingredentTwo.GetComponent<Image>().sprite = recipes[recipePos].ingredentsTwo;
            result.GetComponent<Image>().sprite = recipes[recipePos].result;
            switch (recipePos)
            {
                case 0:
                    IsHealPotion = true;
                    IsManaPotion = false;
                    IsHealPotionPlus = false;

                    break;
                case 1:
                    IsHealPotion = false;
                    IsManaPotion = true;
                    IsHealPotionPlus = false;

                    break;
                case 2:
                    IsHealPotion = false;
                    IsManaPotion = false;
                    IsHealPotionPlus = true;


                    break;
                case 3:
                    IsHealPotion = false;
                    IsManaPotion = false;
                    IsHealPotionPlus = false;

                    break;//venom and leaf for Mana+
            }
        }
    }

    void CraftPotion()
    {
        if (IsHPPotion() && ingredents.beewaxCount > 0 && ingredents.mushroomCount > 0)
        {
            ingredents.HealthPotion++;
            ingredents.beewaxCount--;
            ingredents.mushroomCount--;

            makePotionsTimer = 0;
        }
        else if (IsMPPotion() && ingredents.beewaxCount > 0 && ingredents.venomGlandCount > 0)
        {
            ingredents.ManaPotion++;
            ingredents.beewaxCount--;
            ingredents.venomGlandCount--;
        }
        else if (IsHPPotionPlus() && ingredents.beewaxCount > 0 && ingredents.leafCount > 0)
        {
            ingredents.ManaPotion++;
            ingredents.beewaxCount--;
            ingredents.leafCount--;
        }
        else if (IsMPPotion() && ingredents.venomGlandCount > 0 && ingredents.leafCount > 0)
        {
            ingredents.ManaPotion++;
            ingredents.venomGlandCount--;
            ingredents.leafCount--;
        }
        InventorySystem.instance.VerifyCount();
        gameManager.instance.UpdatePotionCount();
    }

    public bool IsHPPotion()
    {
        return IsHealPotion;
    }

    public bool IsMPPotion()
    {
        return IsManaPotion;
    }

    public bool IsHPPotionPlus()
    {
        return IsHealPotionPlus;
    }
}
 */

//Pickup code
/*
using System.Collections;
using UnityEngine;

public class pickup : MonoBehaviour
{
	enum pickupType { weapon, item }
	[SerializeField] pickupType type;

	[SerializeField] spellStats spell;
	[SerializeField] itemStats item;

	[SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audPick;
	[Range(0, 10)][SerializeField] float audPickVol;
	//bool isFirstTimePickedup;
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player") && aud != null)
        {
            aud.PlayOneShot(audPick[Random.Range(0, audPick.Length)], audPickVol);
        }
        IPickup toPickup = other.GetComponent<IPickup>();

		if (toPickup != null)
		{
			if (type == pickupType.weapon)
			{
				//Debug.Log("Picked up a weapon");
				toPickup.GetSpellStats(spell);
			}
			else if (type == pickupType.item)
			{
				//Debug.Log("Picked up an item");
				toPickup.GetItemStats(item);
			}
			
		}
	}
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            aud.PlayOneShot(audPick[Random.Range(0, audPick.Length)], audPickVol);
            Destroy(gameObject);
        }
    }
}
 */

//IPickup code
/*
using UnityEngine;

public interface IPickup
{
	public void GetSpellStats(spellStats spell);
	public void GetItemStats(itemStats item);
}
 */

//Selection Spawner code
/*
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectionSpawner : MonoBehaviour
{
    public static SelectionSpawner instance;
    enum spawntype { Enemies, PickUps, Fairy }
    [Header("Type Lists")]
    [SerializeField] spawntype type;
    [SerializeField] List<spawnStats> spawnList = new List<spawnStats>();
    [SerializeField] List<spellStats> spellList = new List<spellStats>();
    [SerializeField] List<itemStats> itemList = new List<itemStats>();

    [Header("Fairy")]
    public List<string> dialogue;
    int dialogueCount;
    [SerializeField] GameObject FairySpawner;
    [SerializeField] GameObject Fairy;
    GameObject cloneFairy;

    [Header("Spawner")]
    GameObject spawnObject;
    [SerializeField] int numToSpawn;
    [SerializeField] int spawnRate;
    [SerializeField] Transform[] spawnPos;
    int objectListPos = 0;

    [Header("Selector Display")]
    [SerializeField] GameObject Canvas;
    [SerializeField] Image image;

    [SerializeField] GameObject leftButtonFilled;
    [SerializeField] GameObject leftButtonHole;
    [SerializeField] GameObject rightButtonFilled;
    [SerializeField] GameObject rightButtonHole;

    int spawnCount;
    float spawnTimer;
    bool playerInTrigger;
    bool startSpawner;

    [SerializeField] float buttonTime;
    float buttonTimer;

    void Start()
    {
        instance = this;
        if (spawnList != null && type == spawntype.Enemies || type == spawntype.PickUps)
        {
            spawnObject = spawnList[objectListPos].pickup;
            //spawnObject = spellList[objectListPos].model;

            if (spawnList[objectListPos].sprite != null)
                image.sprite = spawnList[objectListPos].sprite;

            leftButtonFilled.SetActive(true);
            leftButtonHole.SetActive(false);
            rightButtonFilled.SetActive(true);
            rightButtonHole.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        buttonTimer += Time.deltaTime;
        if (playerInTrigger)
        {
            if (type == spawntype.Enemies)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    startSpawner = true;
                }

                if (Input.GetKeyDown("q"))
                {
                    playerController.instance.enabled = true;
                    playerController.instance.mainCam.gameObject.GetComponent<cameraController>().enabled = true;
                }

                if (buttonTimer >= buttonTime)
                    if (leftButtonFilled.activeSelf == false || rightButtonFilled.activeSelf == false)
                    {
                        leftButtonFilled.SetActive(true);
                        leftButtonHole.SetActive(false);
                        rightButtonFilled.SetActive(true);
                        rightButtonHole.SetActive(false);
                        buttonTimer = 0;
                    }
            }

            if (type == spawntype.PickUps)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    spawn();
                }

                if (Input.GetKeyDown("q"))
                {
                    playerController.instance.enabled = true;
                    playerController.instance.mainCam.gameObject.GetComponent<cameraController>().enabled = true;
                }

                if (buttonTimer >= buttonTime)
                    if (leftButtonFilled.activeSelf == false || rightButtonFilled.activeSelf == false)
                    {
                        leftButtonFilled.SetActive(true);
                        leftButtonHole.SetActive(false);
                        rightButtonFilled.SetActive(true);
                        rightButtonHole.SetActive(false);
                        buttonTimer = 0;
                    }
            }

            if (type == spawntype.Fairy && dialogue.Count != 0)
            {
                if (spawnCount < numToSpawn)
                {
                    spawn();
                    gameManager.instance.DisplayDialogue(dialogue[dialogueCount]);
                    dialogueCount++;
                }

                if (Input.GetButtonDown("Submit") && dialogueCount < dialogue.Count)
                {
                    gameManager.instance.DisplayDialogue(dialogue[dialogueCount]);
                    dialogueCount++;
                }
                else if (Input.GetButtonDown("Submit") && dialogueCount == dialogue.Count)
                {
                    playerController.instance.enabled = true;
                    playerController.instance.mainCam.gameObject.GetComponent<cameraController>().enabled = true;
                    Destroy(cloneFairy);
                    gameManager.instance.HideDialogue();
                    gameEventManager.instance.EventOff(FairySpawner);
                    dialogueCount = 0;
                }
            }
            else if (type == spawntype.Fairy && dialogue.Count == 0)
            {
                playerController.instance.enabled = true;
            }

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnRate && spawnCount < numToSpawn && startSpawner)
            {
                spawn();
            }
            selectEverything();
        }
        else
        {
            startSpawner = false;
            spawnCount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteraction interaction = other.GetComponent<IInteraction>();
        if (interaction != null)
        {
            if (type == spawntype.Enemies || type == spawntype.PickUps)
            {
                Canvas.SetActive(true);
            }

            playerInTrigger = true;
            playerController.instance.mainCam.gameObject.GetComponent<cameraController>().enabled = false;
            playerController.instance.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteraction interaction = other.GetComponent<IInteraction>();
        if (interaction != null)
        {
            if (type == spawntype.Enemies || type == spawntype.PickUps)
            {
                Canvas.SetActive(false);
            }

            playerInTrigger = false;
        }
    }

    void selectEverything()
    {
        if (type == spawntype.Enemies || type == spawntype.PickUps)
        {
            if (Input.GetKeyDown("right") && objectListPos < spawnList.Count - 1)
            {
                rightButtonFilled.SetActive(false);
                rightButtonHole.SetActive(true);
                objectListPos++;
                changeEverything();
            }
            if (Input.GetKeyDown("left") && objectListPos > 0)
            {
                leftButtonFilled.SetActive(false);
                leftButtonHole.SetActive(true);
                objectListPos--;
                changeEverything();
            }
        }
    }

    void changeEverything()
    {
        spawnObject = spawnList[objectListPos].pickup;

        if (spawnList[objectListPos].sprite != null)
            image.sprite = spawnList[objectListPos].sprite;
    }

    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);

        if (type == spawntype.Enemies || type == spawntype.PickUps)
        {
            Instantiate(spawnObject, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        }
        if (type == spawntype.Fairy)
        {
            cloneFairy = Instantiate(Fairy, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        }

        spawnCount++;
        spawnTimer = 0;
    }
}
 */

//IDamage code
/*
using UnityEngine;

public interface IDamage
{
	void TakeDMG(int amount);
}
 */

//Damage code
/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Damage : MonoBehaviour
{
	enum damagetype { moving, stationary, DOT, homing, contact, AOE}
	enum effecttype { none, linger }

    [Header("Types")]
    [SerializeField] damagetype type;
	[SerializeField] effecttype effect;

	[SerializeField] int damageAmount;
	[SerializeField] int damageRate;
	[SerializeField] int speed;
	[SerializeField] float destroyTime;

    [Header("Contact")]
    [SerializeField] int contactDMGAmount;
	[SerializeField] float knockBackStrength;
	[SerializeField] float knockbackDelay;

    [Header("AOE")]
    [SerializeField] GameObject explosionArea;

	[Header("Effect")]
	[SerializeField] float lingerDuration;

    [Header("")]
    [SerializeField] Rigidbody rb;
    bool isDamaging;
	bool canKnockBack = true;
	bool isExploded = false;

    int bounceCount;
	HashSet<Transform> bounceEnemies = new HashSet<Transform>();
	Transform curEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		if (type == damagetype.moving || type == damagetype.homing || type == damagetype.AOE)
		{
			Destroy(gameObject, destroyTime);
			if (type == damagetype.moving || type == damagetype.AOE)
			{
				rb.linearVelocity = transform.forward * speed;
			}
		}
        if (type == damagetype.DOT && effect == effecttype.linger)
        {
			StartCoroutine(lingerEffectDectection());
        }
    }

	// Update is called once per frame
	void Update()
	{
        if (type == damagetype.homing)
		{
			rb.linearVelocity = (gameManager.instance.transform.position - transform.position).normalized * speed * Time.deltaTime;
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		if (!canKnockBack)
		{
			return;
		}
        if (other.isTrigger)
		{
			return;
		}

		IDamage dmg = other.GetComponent<IDamage>();

		if (dmg != null && (type == damagetype.moving || type == damagetype.stationary || type == damagetype.homing))
		{
			dmg.TakeDMG(damageAmount);
		}

		if (type == damagetype.moving || type == damagetype.homing)
		{
			Destroy(gameObject);
		}
        if (other.CompareTag("Player") && type == damagetype.contact )
        {
            dmg.TakeDMG(contactDMGAmount);
            StartCoroutine(PlayerKnockBack(other.transform));
            StartCoroutine(Cooldown());
        }
		if(type == damagetype.AOE)
		{
            if (isExploded)
            {
                return;
            }
            Explode();
        }

    }

	private void OnTriggerStay(Collider other)
	{
		if (other.isTrigger)
		{
			return;
		}
		IDamage dmg = other.GetComponent<IDamage>();
		if (dmg != null && type == damagetype.DOT && effect == effecttype.none)
		{
			if (!isDamaging)
			{
				StartCoroutine(damageOther(dmg));
			}

		}
        if (dmg != null && type == damagetype.DOT && effect == effecttype.linger)
        {
            if (!isDamaging)
            {
                StartCoroutine(lingerEffect(dmg));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
		isDamaging = false;
    }

	IEnumerator lingerEffect(IDamage dmg)
	{
        isDamaging = true;
        dmg.TakeDMG(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
        yield return new WaitForSeconds(lingerDuration);
		Destroy(gameObject);
    }
    void Explode()
	{
		isExploded = true;
		speed = 0;
        rb.linearVelocity = transform.forward * speed;
		rb.useGravity = false;
        explosionArea.SetActive(true);
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
		Destroy(gameObject, destroyTime);
	}
	IEnumerator damageOther(IDamage d)
	{
		isDamaging = true;
		d.TakeDMG(damageAmount);
		yield return new WaitForSeconds(damageRate);
		isDamaging = false;

	}
	IEnumerator PlayerKnockBack(Transform playerPosition)
	{
		canKnockBack = false;
		Vector3 direction = (playerPosition.position - transform.position).normalized;
		float move = 0f;
		while (move < knockBackStrength)
		{
			float range = knockBackStrength * Time.deltaTime;
			playerPosition.Translate(direction * range, Space.World);
			move += range;
			yield return null;
		}
	}
	IEnumerator Cooldown()
	{
		canKnockBack = false;
		yield return new WaitForSeconds (knockbackDelay);
		canKnockBack = true;
	}
    IEnumerator lingerEffectDectection()
    {
        yield return new WaitForSeconds(lingerDuration);
		Destroy(gameObject);
    }
}
 */