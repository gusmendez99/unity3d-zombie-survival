using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AdvancedShooterKit.Events;

public class zombie : MonoBehaviour
{
    public float fieldOfViewAngle = 110f;
    public bool playerInSight;
    public GameObject player;

    private NavMeshAgent nav;
    private SphereCollider col;
  
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        col = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (playerInSight)
        {
            nav.SetDestination(player.transform.position);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player) {
            playerInSight = false;

            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up *.2f, direction.normalized, out hit, col.radius))
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                    }
                }
            }
        }
    }
}
