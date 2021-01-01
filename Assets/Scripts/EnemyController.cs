using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;

    UnityEngine.AI.NavMeshAgent navMeshAgent;

    void Start(){
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        player = GameObject.FindWithTag("Player");
    }

    void Update(){
        if(ShouldUpdateDestination()){
            UpdateDestination();
        }
    }
    private bool ShouldUpdateDestination(){
        // If the enemy can detect where the player is
        return Mathf.Abs(transform.position.y - player.transform.position.y) < 0.5;
    }
    private void UpdateDestination(){
        navMeshAgent.SetDestination(player.transform.position);
    }
}
