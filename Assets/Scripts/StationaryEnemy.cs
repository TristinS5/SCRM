using UnityEngine;

public class StationaryEnemys : MonoBehaviour
{
    [SerializeField] GameObject Enemy;
    [SerializeField] GameObject Projectile;
    [SerializeField] float shootTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //enemy rotates towards player
        Enemy.transform.LookAt(playerController.instance.transform);

        shootTimer -= Time.deltaTime;
        if(shootTimer <= 0)
        {
            Instantiate(Projectile, Enemy.transform.position + Enemy.transform.forward * 2, Enemy.transform.rotation);
        }
    }
}
