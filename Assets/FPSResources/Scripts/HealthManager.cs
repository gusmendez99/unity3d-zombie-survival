using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
	Animator animator;

	[SerializeField] private float health = 100.0f; // health of the character

	public bool isPlayer = false; // to identify if it is a player
	public bool removeColliderOnDeath = false;
	public HealthManager referer;	// Special prorperty for create multiple hit system, if it sets on GameObject that has same HealthManager, Apply Damage to it.
	public float damageFactor = 1.0f; // the factor of damage the character will receive
	public Text healthText;

	void Start() {
        //animator gets the Animator component
		animator = GetComponent<Animator>();
        //if it is a player,then search for the health text
		if(isPlayer) healthText = GameObject.Find("UI/InGameUI/CharacterStatus/HealthText").GetComponent<Text>();
	}

	void Update() {
        // if healthText != null (it is a player) then show the health in each update
		if(healthText) {
			healthText.text = "HP: " + health.ToString();
		}
	}
	// Applies damage to the character
    // damage is the float of damage to apply
	public void ApplyDamage(float damage) {
        // if character isDead then do nothing
		if(IsDead) return;
        // damage * damage factor
		damage *= damageFactor;

		if(referer) {
			referer.ApplyDamage(damage);
		}
		else {
            // update health status
			health -= damage;
            //if health is less than 0
			if(health <= 0) {
                //health = 0
				health = 0;
				//show dead animation
				if(animator) {
					animator.SetTrigger("Dead");
				}
				if(removeColliderOnDeath) {
					RemoveColliders(GetComponents<Collider>());
					RemoveColliders(GetComponentsInChildren<Collider>());
				}
			}
		}
	}
    //sets the health to the character
	public void SetHealth(float newHealth) {
		health = newHealth;
	}
    // returns character state
	public bool IsDead {
		get {
			if(!referer) {
				return health <= 0;
			}
			else {
				return referer.IsDead;
			}
		}
	}
    //remove colliders
	void RemoveColliders(Collider[] colliders) {
		foreach(Collider collider in colliders) {
			collider.enabled = false;
		}
	}
}
