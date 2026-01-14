using UnityEngine;

public class TimedDestroyObject : MonoBehaviour
{
    [SerializeField] float destroyTime;
    float timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (destroyTime <= timer)
        {
            Destroy(gameObject);
            timer = 0f;
        }
    }
}
