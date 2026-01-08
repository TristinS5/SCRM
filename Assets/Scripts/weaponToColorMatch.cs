using UnityEngine;
using System.Collections;

public class weaponToColorMatch : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] GameObject weaponRequired;
    [SerializeField] playerInfo playerInfo;

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
        Debug.Log("Weapon required: " + weaponRequired.name + " | Current weapon: " + playerInfo.currentWeapon.name);
        if (weaponRequired = playerInfo.currentWeapon)
        {
        HP -= amount;

            if (HP <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(flashBlack());
            }
        }
    }

    IEnumerator flashBlack()
    {
        model.material.color = Color.black;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }
}
