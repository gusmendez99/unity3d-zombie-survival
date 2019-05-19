using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chasing : MonoBehaviour {
	NetworkManager networkManager;
	Animator animator;
	HealthManager healthManager;
	NavMeshAgent agent;
	AudioSource audioSource;
	public GameObject target;
	public float damage = 15.0f; // damage that enemy agent cause on the player
	public bool isAttacking = false; // saves if it is atacking agent or not
	public bool shouldChase = true; // saves if it should chase agent or not
	public bool isInLateUpdate = false; // saves if it is in late update state
	public bool shouldUpdate = true; // saves if it should state or not

	public AudioClip attackSound; //sound to play when attacking
	public AudioClip deathSound; //sound when dead 

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		healthManager = GetComponent<HealthManager>();
		agent = GetComponent<NavMeshAgent>();
		audioSource = GetComponent<AudioSource>();

		networkManager = GameObject.Find("GameManager").GetComponent<NetworkManager>();
	}

	IEnumerator distUpdateCo = null;

	// Update is called once per frame
	void Update () {
		if(!shouldUpdate) return;

        //if it is not dead
		if(!healthManager.IsDead) {
			if(!isAttacking) {
				// NetworkPlayer targetNetworkPlayer = target.GetComponent<NetworkPlayer>();

				// if(targetNetworkPlayer.IsLocalPlayer) {
					float distance = GetActualDistanceFromTarget();

					// Reduce calculation of path finding
					if(distance <= 20f) {
						if(distUpdateCo != null) {
							StopCoroutine(distUpdateCo);
						}

						isInLateUpdate = false;
						agent.destination = target.transform.position;
					}
					else if(!isInLateUpdate) {
						if(distance <= 40f) {
							distUpdateCo = LateDistanceUpdate(2f);
							StartCoroutine(distUpdateCo);
						}
						else if(distance <= 60) {
							distUpdateCo = LateDistanceUpdate(3f);
							StartCoroutine(distUpdateCo);
						}
						else if(distance <= 80) {
							distUpdateCo = LateDistanceUpdate(4f);
							StartCoroutine(distUpdateCo);
						}
						else {
							distUpdateCo = LateDistanceUpdate(5f);
							StartCoroutine(distUpdateCo);
						}
					}
				// }
			}

			animator.SetFloat("SpeedMultiplier", agent.speed);

			if(agent.pathPending) return;
            //check if zombie should atack
			CheckAttack();
		}
		else {
            // if zombie is dead then
            // stop updating and chasing
			shouldUpdate = false;
			shouldChase = false;
			//play dead sound
			audioSource.PlayOneShot(deathSound);
            //destroy agent
			Destroy(agent);
            //remove the gameObject
			StartCoroutine(RemoveGameObject());
			return;
		}
	}

	IEnumerator LateDistanceUpdate(float duration) {
		isInLateUpdate = true;
		agent.destination = target.transform.position;
		yield return new WaitForSeconds(duration);
		
		isInLateUpdate = false;
		distUpdateCo = null;
		yield break;
	}
    //get distance from target
	float GetActualDistanceFromTarget() {
		return GetDistanceFrom(target.transform.position, this.transform.position);
	}
    //get distance from a specific point
	float GetDistanceFrom(Vector3 src, Vector3 dist) {
		return Vector3.Distance(src, dist);
	}
    //original speed
	float origSpeed;

	void CheckAttack() {
		// Calculate actual distance from target
		float distanceFromTarget = GetActualDistanceFromTarget();
		
		// Calculate direction is toward player
		Vector3 direction = target.transform.position - this.transform.position;
		float angle = Vector3.Angle(direction, this.transform.forward);

        //if agent is not atacking and distance from target is minimal and angle between target is less than 60 degrees
		if(!isAttacking && distanceFromTarget <= 2.0f && angle <= 60f) {
            // change isAttacking state 
			isAttacking = true;
            // shouldChase = false causes agent to stop chasing
			shouldChase = false;

            //originalSpeed update
			origSpeed = agent.speed;
            // speed = 0 stops the agent 
			agent.speed = 0;

            // play attackSound
			audioSource.PlayOneShot(attackSound);
            // play Attack animation
            animator.SetTrigger("Attack");

            // gets the target (AKA player healthManager)
			HealthManager targetHealthManager = target.GetComponent<HealthManager>();

            //if targetHealthManager != null then apply damage to targer
			if(targetHealthManager) {
				targetHealthManager.ApplyDamage(damage);
			}
            // call resetAttacking()
			StartCoroutine(ResetAttacking());
		}
	}
    // ResetAttacking() resets all attacking related variables
	IEnumerator ResetAttacking() {
        //waits for 1.4 seconds
		yield return new WaitForSeconds(1.4f);
        // agent is not attacking eanymore
		isAttacking = false;
        // agent should start chasing again
		shouldChase = true;

        // if agent is not dead
		if(!healthManager.IsDead) {
            // agent.speed is the saved origSpeed
			agent.speed = origSpeed;
		}

		
		yield break;
	}

	IEnumerator RemoveGameObject() {
		yield return new WaitForSeconds(5f);
		// PhotonNetwork.Destroy(gameObject);
		Destroy(gameObject);
	}
}
