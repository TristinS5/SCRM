using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] BoxCollider FinishLineBox;
    bool FinishLineActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (FinishLineActive)
                return;

            FinishLineActive = true;
            Timer.instance.StopTimer();
            Gamemanager.instance.StateWin();
        }
    }
}
