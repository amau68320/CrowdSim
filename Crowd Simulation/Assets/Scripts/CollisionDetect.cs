using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollisionDetect : MonoBehaviour
{
    public bool isInFront = false;
    private float fixTimeColliding;
    private float timeColliding;
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;

    void Start()
    {
        fixTimeColliding = Random.Range(1.0f, 3.0f);
        timeColliding = fixTimeColliding;
        agent = gameObject.GetComponentInParent<NavMeshAgent>();
        obstacle = gameObject.GetComponentInParent<NavMeshObstacle>();
    }
    void OnTriggerEnter(Collider character)
    {
        isInFront = true;
    }

    void OnTriggerExit(Collider character)
    {
        isInFront = false;
    }

    void Update()
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
                    gameObject.GetComponentInParent<AgentManager>().hasToWait = true;
            }
        }
        else
        {
            timeColliding = fixTimeColliding;
        }

    }
}
