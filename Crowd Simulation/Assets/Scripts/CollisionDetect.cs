using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollisionDetect : MonoBehaviour
{
    private bool isInFront = false;
    private float timeColliding = 2f;
    private GameObject collidingPerso;
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private int priority;

    void Start()
    {
        agent = gameObject.GetComponentInParent<NavMeshAgent>();
        obstacle = gameObject.GetComponentInParent<NavMeshObstacle>();
        priority = agent.avoidancePriority;
    }
    void OnTriggerEnter(Collider character)
    {
        collidingPerso = character.gameObject;
        isInFront = true;
    }

    void OnTriggerExit(Collider character)
    {
        if (collidingPerso == character.gameObject) ;
            isInFront = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(!obstacle.enabled)
        {
            if (isInFront)
                timeColliding -= Time.deltaTime;
            else
                timeColliding = 2f;

            if (timeColliding <= 0)
            {
                if (collidingPerso.gameObject.GetComponent<NavMeshAgent>().avoidancePriority < priority)
                {
                    collidingPerso.GetComponent<NavMeshAgent>().enabled = false;
                    collidingPerso.GetComponent<NavMeshObstacle>().enabled = true;
                    Animator animator = collidingPerso.GetComponent<Animator>();
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isWaiting", true);
                }
                else
                {
                    Animator animator = gameObject.GetComponentInParent<Animator>();
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isWaiting", true);
                    agent.enabled = false;
                    obstacle.enabled = true;
                }
            }
        }

    }
}
