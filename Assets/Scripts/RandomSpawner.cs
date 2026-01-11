using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> wantToSpawnList = new List<GameObject>();
    //public GameObject wantToSpawn;
    public BoxCollider spawnArea;

    float spawnTimer;
    bool playerInTrigger;
    bool startSpawner;
    [SerializeField] int spawnRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger)
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

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnRate && startSpawner)
            {
                Spawn();
                spawnTimer = 0f;
            }
        }
        else
        {
            startSpawner = false;
        }
    }

    void Spawn()
    {
        Vector3 center = spawnArea.bounds.center;// + transform.position;
        Vector3 size = spawnArea.bounds.size;

        GameObject wantToSpawn = wantToSpawnList[Random.Range(0, wantToSpawnList.Count)];

        Vector3 randomPos = new Vector3(
            Random.Range(center.x - size.x / 2, center.x + size.x / 2),
            Random.Range(center.y - size.y / 2, center.y + size.y / 2),
            Random.Range(center.z - size.z / 2, center.z + size.z / 2)
        );

        Instantiate(wantToSpawn, randomPos, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteraction interaction = other.GetComponent<IInteraction>();
        if (interaction != null)
        {
            playerInTrigger = true;
            playerController.instance.enabled = false;
        }
    }
}
