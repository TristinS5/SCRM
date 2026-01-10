using System.Collections;
using UnityEngine;

public class pickup : MonoBehaviour
{
	enum pickupType { weapon, item }
	[SerializeField] pickupType type;

	[SerializeField] WeaponStats weapon;
	//[SerializeField] itemStats item;

	[SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audPick;
	[Range(0, 10)][SerializeField] float audPickVol;
	//bool isFirstTimePickedup;
    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
	{
        //if (other.CompareTag("Player") && aud != null)
        //{
        //    aud.PlayOneShot(audPick[Random.Range(0, audPick.Length)], audPickVol);
        //}

        IPickup toPickup = other.GetComponent<IPickup>();

		
		if (toPickup != null) //checks to see if the other object has the IPickup
        {
			if (type == pickupType.weapon) //checks if the pickup is a weapon
            {
				toPickup.GetWeaponStats(weapon);
			}
			//else if (type == pickupType.item)
			//{
			//	//Debug.Log("Picked up an item");
			//	toPickup.GetItemStats(item);
			//}
			
			Destroy(gameObject);
		}
	}

	//private void OnTriggerExit(Collider other) // After Picking up and walking away
	//{
	//	if (other.CompareTag("Player"))
	//	{
	//		//aud.PlayOneShot(audPick[Random.Range(0, audPick.Length)], audPickVol);
	//		//Destroy(gameObject);
	//	}
	//}
}
