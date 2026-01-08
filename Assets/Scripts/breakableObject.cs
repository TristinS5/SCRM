using UnityEngine;
using System.Collections;

public class breakableObject : MonoBehaviour,IDamage
{
    [SerializeField] Renderer model;

    [SerializeField] int HP;

    Color colorOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            Debug.Log("Destroy in BreakableObject");
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashBlack());
        }
    }

    IEnumerator flashBlack()
    {
        model.material.color = Color.black;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }
}
