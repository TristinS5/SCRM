using UnityEngine;
using System.Collections;

public class weaponToColorMatch : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] WeaponStats weaponStatsRequired;
    [SerializeField] WeaponStats weaponStatsRequired2;
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
        Debug.Log("Weapon required: " + weaponStatsRequired.model.name + " | Current weapon: " + playerInfo.currentWeapon.name);
        Debug.Log("GameObject Name " + gameObject.name);
        if (weaponStatsRequired.model == playerInfo.currentWeapon || weaponStatsRequired2.model == playerInfo.currentWeapon)
        {
        HP -= amount;

            if (HP <= 0)
            {
                gameObject.SetActive(false);
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
