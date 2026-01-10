using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SelectionSpawner : MonoBehaviour
{
    public static SelectionSpawner instance;
    enum spawntype { Enemies, PickUps, Fairy }
    [Header("Type Lists")]
    [SerializeField] spawntype type;
    //[SerializeField] List<spawnStats> spawnList = new List<spawnStats>();
    [SerializeField] List<WeaponStats> weaponsList = new List<WeaponStats>();
    //[SerializeField] List<itemStats> itemList = new List<itemStats>();

    //[Header("Fairy")]
    //public List<string> dialogue;
    //int dialogueCount;
    //[SerializeField] GameObject FairySpawner;
    //[SerializeField] GameObject Fairy;
    //GameObject cloneFairy;

    [Header("Spawner")]
    GameObject spawnObject;
    [SerializeField] int numToSpawn;
    [SerializeField] int spawnRate;
    [SerializeField] Transform[] spawnPos;
    int objectListPos = 0;

    [Header("Selector Display")]
    [SerializeField] GameObject Canvas;
    [SerializeField] Image image;
    [SerializeField] TMP_Text weaponName;

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
        if (weaponsList != null && type == spawntype.Enemies || type == spawntype.PickUps)
        {
            spawnObject = weaponsList[objectListPos].model;
            Debug.Log("Changed to " + weaponsList[objectListPos].model.name);
            weaponName.text = weaponsList[objectListPos].model.name;

            if (weaponsList[objectListPos].sprite != null)
                image.sprite = weaponsList[objectListPos].sprite;

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
                    playerController.instance.playerCam.gameObject.GetComponent<cameraController>().enabled = true;
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
                    playerController.instance.playerCam.gameObject.GetComponent<cameraController>().enabled = true;
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

            //if (type == spawntype.Fairy && dialogue.Count != 0)
            //{
            //    if (spawnCount < numToSpawn)
            //    {
            //        spawn();
            //        gameManager.instance.DisplayDialogue(dialogue[dialogueCount]);
            //        dialogueCount++;
            //    }

            //    if (Input.GetButtonDown("Submit") && dialogueCount < dialogue.Count)
            //    {
            //        gameManager.instance.DisplayDialogue(dialogue[dialogueCount]);
            //        dialogueCount++;
            //    }
            //    else if (Input.GetButtonDown("Submit") && dialogueCount == dialogue.Count)
            //    {
            //        playerController.instance.enabled = true;
            //        playerController.instance.mainCam.gameObject.GetComponent<cameraController>().enabled = true;
            //        Destroy(cloneFairy);
            //        gameManager.instance.HideDialogue();
            //        gameEventManager.instance.EventOff(FairySpawner);
            //        dialogueCount = 0;
            //    }
            //}
            //else if (type == spawntype.Fairy && dialogue.Count == 0)
            //{
            //    playerController.instance.enabled = true;
            //}

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
            playerController.instance.playerCam.gameObject.GetComponent<cameraController>().enabled = false;
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
            if (Input.GetKeyDown("d") && objectListPos < weaponsList.Count - 1)
            {
                rightButtonFilled.SetActive(false);
                rightButtonHole.SetActive(true);
                objectListPos++;
                changeEverything();
            }
            if (Input.GetKeyDown("a") && objectListPos > 0)
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
        spawnObject = weaponsList[objectListPos].model;

        Debug.Log("Changed to " + weaponsList[objectListPos].model.name);
        weaponName.text = weaponsList[objectListPos].model.name.ToString();

        if (weaponsList[objectListPos].sprite != null)
            image.sprite = weaponsList[objectListPos].sprite;
    }

    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);

        if (type == spawntype.Enemies || type == spawntype.PickUps)
        {
            Instantiate(spawnObject, playerController.instance.transform.position, playerController.instance.transform.rotation);
        }
        //if (type == spawntype.Fairy)
        //{
        //    cloneFairy = Instantiate(Fairy, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        //}

        spawnCount++;
        spawnTimer = 0;
    }
}