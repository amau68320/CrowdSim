using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// the agent behavior during the party will be managed by a state machine
public class AgentManager : MonoBehaviour
{
    private float hunger;
    private float shyness;
    private float moveRate;
    private bool isGoingtoEat;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;

    void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hunger = Random.Range(0.0f, 100.0f);
        shyness = Random.Range(0.0f, 100.0f);
        moveRate = Random.Range(0.0f, 0.001f);
        isGoingtoEat = false;
    }

    void Update()
    {
        if(animator.GetBool("isWaiting"))
        {
            float choice = Random.Range(0.0f, 1.0f);

            // we still waiting 99.9% of the update and depending on a moveRate 
            if(choice >= (0.999f + moveRate))
            {
                if (Random.Range(0.0f, 100f) < shyness)
                {
                    //eloign from group 
                    animator.SetBool("isWaiting", false);
                    animator.Rebind();
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    if (Random.Range(0.0f, 100.0f) < hunger)
                    {
                        // eat
                        animator.SetBool("isWaiting", false);
                        animator.Rebind();
                        animator.SetBool("isWalking", true);
                        obstacle.enabled = false;
                        agent.enabled = true;
                        agent.SetDestination((new Vector3(4.0f, 0.0f, -6.5f)));
                        isGoingtoEat = true;
                    }
                    else
                    {
                        // call someone to talk
                        animator.SetBool("isWaiting", false);
                        animator.Rebind();
                        animator.SetBool("isTalking", true);
                    }
                }
            }
        }
    }
}
