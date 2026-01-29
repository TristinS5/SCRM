using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SpawnItemOnColliderTrigger : MonoBehaviour
{
    bool playerInTrigger = false;
    bool HasSpawnedOnce = false;
    [SerializeField] List<BoxCollider> spawnAreas = new List<BoxCollider>();
    [SerializeField] List<GameObject> wantToSpawnList = new List<GameObject>();
    enum SpawnType
    {
        Yellow,
        Orange,
        Cyan,
        All
    }
    [SerializeField] SpawnType spawnType;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        
        for (int i = 0; i < spawnAreas.Count; i++)
        {
            Vector3 center = spawnAreas[i].bounds.center;// + transform.position;
            Vector3 size = spawnAreas[i].bounds.size;

            GameObject wantToSpawn;
            if (spawnType == SpawnType.All)
            { 
                wantToSpawn = wantToSpawnList[Random.Range(0, wantToSpawnList.Count)]; 
            }
            else
            { 
                wantToSpawn = wantToSpawnList[(int)spawnType]; 
            }

                Vector3 randomPos = new Vector3(
                    Random.Range(center.x - size.x / 2, center.x + size.x / 2),
                    Random.Range(center.y - size.y / 2, center.y + size.y / 2),
                    Random.Range(center.z - size.z / 2, center.z + size.z / 2)
                );

            Instantiate(wantToSpawn, randomPos, Quaternion.identity);
        }
        HasSpawnedOnce = true;
        playerInTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasSpawnedOnce){
            return;
        }
        playerInTrigger = true;
    }
}
