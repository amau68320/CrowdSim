using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// marche pas fdp
public class SpawnWalk : MonoBehaviour
{

    private NavMeshAgent agent;

    // for Initialization
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveToLocation(Vector3 targetPoint)
    {
        agent.destination = targetPoint;
        agent.isStopped = false;
    }
}