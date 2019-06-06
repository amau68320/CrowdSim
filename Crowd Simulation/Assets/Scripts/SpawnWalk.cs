using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

// This script manages the spawn and walk inside the room for each agent
public class SpawnWalk : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
    private float xDest;
    private float zDest;

    // At initialization all agents walk to a random destionation inside the room
    private void Awake()
    {
        xDest = Random.Range(-10.5f, 9.5f);
        zDest = Random.Range(-6.5f, 6.5f);

        obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        animator.SetBool("isWalking", true);
        agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
    }

   private void Update()
    {
        // has reach its destination position test
        if (agent.enabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.SetBool("isWalking", false);
                    animator.Rebind();
                    animator.SetBool("isWaiting", true);
                    agent.enabled = false;
                    obstacle.enabled = true;
                }
            }
        }

        // In case agent stop outside the room (because of the security collider) 
        if(obstacle.enabled && ((this.gameObject.transform.position.x < -10.7f) || (this.gameObject.transform.position.x > 9.7f) || (this.gameObject.transform.position.z <-6.7f) || (this.gameObject.transform.position.z > 6.7f)))
        {
            animator.SetBool("isWaiting", false);
            animator.Rebind();
            animator.SetBool("isWalking", true);
            obstacle.enabled = false;
            agent.enabled = true;
            agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
        }
        else if(obstacle.enabled)
        {
            agent.speed = 0.75f;
            gameObject.GetComponent<AgentManager>().enabled = true; // we stop this script and activate the reception manager script
            this.enabled = false;
        }
    }

}
