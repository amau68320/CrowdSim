using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 
// marche pas fdp
public class SpawnWalk : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    // for Initialization
    void Awake()
    {
        float x = Random.Range(-11f, 10f);
        float z = Random.Range(-6.7f, 6.8f);

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(new Vector3(x, this.gameObject.transform.position.y, z));
    }

    void Update()
    {
        // has reach its final position test
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.SetTrigger("StopWalk");
                }
            }
        }
    }

}