using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This script manage a invisible trigger objet that allows a security during enter and reception
// In fact, the agents don't detect other active agents ("the ones who walk") so this script allows them to let the other walk by stopping the agent
public class CollisionDetect : MonoBehaviour
{
    public bool isInFront = false;
    private float fixTimeColliding;
    private float timeColliding;
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;

    private void Start()
    {
        // agent own time to decide to let an other agent walk
        fixTimeColliding = Random.Range(0.5f, 2.0f);
        timeColliding = fixTimeColliding;
        agent = gameObject.GetComponentInParent<NavMeshAgent>();
        obstacle = gameObject.GetComponentInParent<NavMeshObstacle>();
    }
    private void OnTriggerEnter(Collider character)
    {
        isInFront = true;
    }

    void OnTriggerExit(Collider character)
    {
        isInFront = false;
    }

    private void Update()
    {
        if(!obstacle.enabled)
        {
            if (isInFront)
                timeColliding -= Time.deltaTime;
            else
                timeColliding = fixTimeColliding;

            if (timeColliding <= 0)
            {
                Animator animator = gameObject.GetComponentInParent<Animator>();
                animator.SetBool("isWalking", false);
                animator.Rebind();
                animator.SetBool("isWaiting", true);
                agent.enabled = false;
                obstacle.enabled = true;
                timeColliding = fixTimeColliding;

                if (gameObject.GetComponentInParent<AgentManager>().enabled)
                    gameObject.GetComponentInParent<AgentManager>().SetHasToWait(true); // notify the agent that he must wait a time defined in AgentManager.cs before going back to its activity
            }
        }
        else
        {
            timeColliding = fixTimeColliding;
        }

    }
}
