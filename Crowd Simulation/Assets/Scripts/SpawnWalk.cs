using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class SpawnWalk : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
    private float xDest;
    private float zDest;

    // for Initialization
    void Awake()
    {
        xDest = Random.Range(-11f, 10f);
        zDest = Random.Range(-6.7f, 6.8f);

        obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
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
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isWaiting", true);
                    agent.enabled = false;
                    obstacle.enabled = true;
                }
            }
        }

        if(obstacle.enabled && ((this.transform.position.x < -11f) || (this.transform.position.x > 10f) || (this.transform.position.z <-6.7f) || (this.transform.position.z > 6.8f)))
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isWaiting", false);
            agent.enabled = true;
            obstacle.enabled = false;
            animator.SetTrigger("ReWalk");
            agent.ResetPath();
            agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
        }
    }

}
