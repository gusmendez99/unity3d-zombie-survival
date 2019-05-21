using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Weapon {
	None,    // names of all the posible weapons the player can use
	Glock,
	AKM,
	MP5K,
	M870
};

public class WeaponManager : MonoBehaviour {
	[SerializeField]
	public Weapon primaryWeapon; // primary weapon the user will use

    [SerializeField]
    public Weapon secondaryWeapon; // secondary weapon the user will use

	public Weapon currentWeapon; // the current weapon the user will use

	public GameObject primaryWeaponGO;
	public GameObject secondaryWeaponGO;
	public GameObject currentWeaponGO;

	void Start() {
		primaryWeapon = Weapon.None; 
		secondaryWeapon = Weapon.Glock; 
        currentWeapon = secondaryWeapon; // initialize with the glock weapon 

        primaryWeaponGO = null; // not primary weapon GameObject assigned
		secondaryWeaponGO = transform.Find(secondaryWeapon.ToString()).gameObject; // Finds the GameObject that match the name
		currentWeaponGO = secondaryWeaponGO; //assign the secondaryWeaponGO to the currentWeaponGO

		StartCoroutine(Init()); // starts init()
	}

	IEnumerator Init() {
		currentWeaponGO.SetActive(true); // sets the currentWeaponGO active
		
		yield return new WaitForSeconds(0.1f); //waits for a minimum amount of time
		currentWeaponGO.GetComponent<WeaponBase>().Draw(); // then it is called the WeaponBase component to draw the weapon 

		yield break;
	}

	void Update() {
		if(primaryWeapon != Weapon.None && Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon != primaryWeapon) { 
            //Checks if the user has pressed keyCode Alpha 1 and if that gun exists in the player's inventory

            // if it exists, unloads current weapon by calling the WeaponBase component
			currentWeaponGO.GetComponent<WeaponBase>().Unload();
            // assigns the primary weapon as current weapon and the primaryWeaponGo as CurrentWeaponGo
			currentWeapon = primaryWeapon;
			currentWeaponGO = primaryWeaponGO;

			currentWeaponGO.SetActive(true);
			currentWeaponGO.GetComponent<WeaponBase>().Draw();
		}
		else if(secondaryWeapon != Weapon.None && Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon != secondaryWeapon) {
			currentWeaponGO.GetComponent<WeaponBase>().Unload();

			currentWeapon = secondaryWeapon;
			currentWeaponGO = secondaryWeaponGO;

			currentWeaponGO.SetActive(true);
			currentWeaponGO.GetComponent<WeaponBase>().Draw();
		}
	}

	public bool HasWeapon(Weapon weapon) {
		if(primaryWeapon == weapon || secondaryWeapon == weapon) return true;
		
		return false;
	}
}
