﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum States
{
    WAITING = 0,
    GOINGTOEAT = 1,
    EATING = 2,
    TALKING = 3,
    GOINGTOTALK = 4
}

// the agent behavior during the party will be managed by a state machine
public class AgentManager : MonoBehaviour
{
    private States currentState;
    private float hunger;
    private float shyness;
    private float moveRate;
    private float xDest;
    private float zDest;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;

    void Start()
    {
        currentState = States.WAITING;
        obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hunger = Random.Range(0.0f, 100.0f);
        shyness = Random.Range(0.0f, 100.0f);
        moveRate = Random.Range(0.0f, 0.001f);
    }

    void Update()
    {
        if(currentState == States.WAITING)
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
                        SelectTable();
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
        else if(currentState == States.GOINGTOEAT)
        {
            CheckReachTable();
        }
    }

    void CheckReachTable()
    {
        if (agent.enabled)
        {
            if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.SetBool("isWalking", false);
                    animator.Rebind();
                    animator.SetBool("isEating", true);
                    agent.enabled = false;
                    obstacle.enabled = true;
                    currentState = States.EATING;
                }
            }
        }
    }

    void SelectTable()
    {
        animator.SetBool("isWaiting", false);
        animator.Rebind();
        animator.SetBool("isWalking", true);
        obstacle.enabled = false;
        agent.enabled = true;

        switch(Random.Range(0,5))
        {
            case 0:
                xDest = 4.0f;
                zDest = -6.5f;
                break;

            case 1:
                xDest = -4.0f;
                zDest = -6.5f;
                break;

            case 2:
                xDest = -4.0f;
                zDest = 6.9f;
                break;

            case 3:
                xDest = 4.0f;
                zDest = 6.9f;
                break;

            case 4:
                xDest = 0.0f;
                zDest = 0.65f;
                break;
        }
        agent.SetDestination((new Vector3(xDest, 0.0f, zDest)));
        currentState = States.GOINGTOEAT;
    }
}
