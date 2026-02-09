using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class SpawnItemOnColliderTrigger : MonoBehaviour
{
    bool playerInTrigger = false;
    bool HasSpawnedOnce = false;
    [SerializeField] List<BoxCollider> spawnAreas = new List<BoxCollider>();
    [SerializeField] List<GameObject> wantToSpawnList = new List<GameObject>();
    [SerializeField] List<GameObject> SpawnedObjects = new List<GameObject>();
    [SerializeField] GameObject Wall;
    enum SpawnType
    {
        Yellow,
        Orange,
        Cyan,
        All
    }
    [SerializeField] SpawnType spawnType;

    //[SerializeField] LevelStats levelStats;
    //enum LevelType
    //{
    //    Stage1,
    //    Stage2,
    //    Stage3,
    //    Stage4
    //}
    //[SerializeField] LevelType levelType;



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

        for (int i = 0; i < SpawnedObjects.Count; i++)
        {
            if(SpawnedObjects[i] == null)
            {
                SpawnedObjects.RemoveAt(i);
            }
        }

        if(SpawnedObjects.Count == 0 && HasSpawnedOnce)
        {
            //switch((int)levelType)
            //{
            //    case 0:
            //        if(levelStats.isTimed1 == false)
            //        {
            //        levelStats.Stage1Time = Timer.instance.CurrentTime;
            //            levelStats.isTimed1 = true;
            //        }
            //        break;
            //    case 1:
            //        if (levelStats.isTimed2 == false)
            //        {
            //            levelStats.Stage2Time = Timer.instance.CurrentTime - levelStats.Stage1Time;
            //            levelStats.isTimed2 = true;
            //        }
            //        break;
            //    case 2:
            //        if (levelStats.isTimed3 == false)
            //        {
            //            levelStats.Stage3Time = Timer.instance.CurrentTime - levelStats.Stage2Time;
            //            levelStats.isTimed3 = true;
            //        }
            //        break;
            //    case 3:
            //        if (levelStats.isTimed4 == false)
            //        {
            //            levelStats.Stage4Time = Timer.instance.CurrentTime - levelStats.Stage3Time;
            //            levelStats.isTimed4 = true;
            //        }
            //        break;
            //}

            Wall.SetActive(false);
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
            SpawnedObjects.Add(
            Instantiate(wantToSpawn, randomPos, Quaternion.identity));
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
