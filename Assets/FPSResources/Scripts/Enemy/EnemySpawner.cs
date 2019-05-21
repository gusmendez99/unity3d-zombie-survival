using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour {
	[Header("Enemy Spawn Management")]
	public float respawnDuration =20.0f; // spawn a new set of enemies every 20 seconds
	public List<GameObject> spawnPoints = new List<GameObject>(); // list that contains all the points were enemies are spawned
	public GameObject target; //target that enemies will follow (AKA player prefab)
	
	[Header("Enemy Status")]
	public float startHealth = 100f; //start health for all enemies (without upgrade)
	public float startMoveSpeed = 1f; //start moving speed for all enemies (without upgrade)
    public float startDamage = 15f; //start damage each enemy will cause on the player (without upgrade)
    public int startEXP = 3; //start experience given to the player if he kills an e
	public int startFund = 5; //start fund for all enemies
	public float upgradeDuration = 60f;	// Increase all enemy stats every 30 seconds

	private float upgradeTimer; //saves the time spent since last upgrade
	[SerializeField]
	private float currentHealth; //health that will be actualized every time enemies are upgraded
	[SerializeField]
	private float currentMoveSpeed; //moving speed that will be actualized every time enemies are upgraded
    [SerializeField]
	private float currentDamage; //amount of damage caused to the player that will be actualized every time enemies are upgraded
    [SerializeField]
	private int currentEXP; //expierence that will be changed after each upgrade
	[SerializeField]
	private int currentFund; //fund that will be changed after each upgrade

	private NetworkManager networkManager;
	
	
	private float spawnTimer; //saves the time spent since last spawn

	private PrefabManager prefabManager; //prefavManager is in charge of generating new pregabs
	private List<GameObject> enemies = new List<GameObject>(); //list of enemies

    // START DECLARATION OF OWN VARIABLES
    public int enemiesToSpawn = 20; //total of enemies to spawn according to the stage
    private int enemiesSpawned = 0; // total of enemies already spawn


	void Start() {
		currentHealth = startHealth; 
		currentMoveSpeed = startMoveSpeed;
		currentDamage = startDamage;
		currentEXP = startEXP;
		currentFund = startFund;
        //get instance of the prefabManager
		prefabManager = PrefabManager.GetInstance();
        //add a new zombie GameObject to the enemies list by calling the GetPrefav method from the prefabManager
		enemies.Add(prefabManager.GetPrefab("Zombie"));

		networkManager = GameObject.Find("GameManager").GetComponent<NetworkManager>();
	}

	void Update() {
        //compare time passed since last spawnEnemy call to respawn duration
		if(spawnTimer < respawnDuration) {
            //if its minor, it means not enough time has passed to call spawnEnemy again
			spawnTimer += Time.deltaTime; //sum deltaTime to spawnTimer
		}
		else {
            if (enemiesSpawned < enemiesToSpawn)
            {
                //if its greater, it means is time to a new spawnEnemy call
                Debug.Log(enemiesSpawned.ToString());
                SpawnEnemy();
            } else
            {
                Debug.Log("ALL ENEMIES DEAD");
            }
		}
        //compare time passed since last upgradeEnemy call to upgradeTimer duration
        if (upgradeTimer < upgradeDuration) {
            //if its minor, it means not enough time has passed to call upgradeEnemy again
            upgradeTimer += Time.deltaTime;
		}
		else {
            //if its greater, it means is time to a new UogradeEnemy call
            UpgradeEnemy();
		}
	}

	float GetDistanceFrom(Vector3 src, Vector3 dist) {
		return Vector3.Distance(src, dist);
	}

	// GameObject getClosestPlayer(Transform spawnPoint) {
	// 	float minDist = 10000000f;
	// 	GameObject closestTarget = null;
	// 	List<GameObject> players = networkManager.Players;

	// 	foreach(GameObject player in players) {
	// 		float dist = GetDistanceFrom(spawnPoint.position, player.transform.position);
			
	// 		if(dist < minDist) {
	// 			minDist = dist;
	// 			closestTarget = player;
	// 		}
	// 	}

	// 	return closestTarget;
	// }

	void SpawnEnemy() {
        //compare time passed since last spawnEnemy call to respawn duration
        // if its minor then do nothing
        if (spawnTimer < respawnDuration) return;
        
        //for each spawn point
		foreach(GameObject spawnPoint in spawnPoints) {
            //get a zombie from the enemies list and add the current values according to the upgrade
			GameObject zombie = enemies[0];
			zombie.GetComponent<Chasing>().target = target;
			zombie.GetComponent<Chasing>().damage = currentDamage;
			zombie.GetComponent<NavMeshAgent>().speed = currentMoveSpeed;
			zombie.GetComponent<HealthManager>().SetHealth(currentHealth);
			zombie.GetComponent<KillReward>().exp = currentEXP;
			zombie.GetComponent<KillReward>().fund = currentFund;

			// Boost rotating speed
			float rotateSpeed = 120f + currentMoveSpeed;
			rotateSpeed = Mathf.Max(rotateSpeed, 200f);	// Max 200f
			zombie.GetComponent<NavMeshAgent>().angularSpeed = rotateSpeed;

			// PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
			Instantiate(zombie, spawnPoint.transform.position, spawnPoint.transform.rotation);
            enemiesSpawned += 1;
            Debug.Log("ENEMY SPAWNED");

		}
		
		spawnTimer = 0f;
	}
    //UpgradeEnemy() just improves currentHealth, currentMove etc
	void UpgradeEnemy() {
       
		print("ENEMY UPGRADED");

		currentHealth += 5;

		if(currentMoveSpeed < 4f) {
			currentMoveSpeed += 0.2f;
		}
		if(currentDamage < 51f) {
			currentDamage += 2f;
		}
		
		currentEXP++;
		currentFund++;

		upgradeTimer = 0;
    }
}
