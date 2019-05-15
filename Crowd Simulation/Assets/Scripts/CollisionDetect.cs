﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollisionDetect : MonoBehaviour
{
    public bool isInFront = false;
    private float timeColliding;
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;

    void Start()
    {
        timeColliding = Random.Range(1.0f, 3.0f);
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
    // Update is called once per frame
    void Update()
    {
        if(!obstacle.enabled)
        {
            if (isInFront)
                timeColliding -= Time.deltaTime;
            else
                timeColliding = Random.Range(1.5f,3.0f);

            if (timeColliding <= 0)
            {
                Animator animator = gameObject.GetComponentInParent<Animator>();
                animator.SetBool("isWalking", false);
                animator.Rebind();
                animator.SetBool("isWaiting", true);
                agent.enabled = false;
                obstacle.enabled = true;
                timeColliding = Random.Range(3.0f, 5.0f);

                if(gameObject.GetComponentInParent<AgentManager>().enabled)
                    AgentManager.hasToWait = true;
            }
        }

    }
}
