﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum States
{
    WAITING = 0,
    GOINGTOEAT = 1,
    EATING = 2,
    TALKING = 3,
    GOINGTOTALK = 4,
    GOINGAWAY = 5
}

// the agent behavior during the party will be managed by a state machine
public class AgentManager : MonoBehaviour
{
    public  bool hasToWait;
    public  bool isAtTable;
    private bool isSeekingForDistance;
    private float timeToWait;
    private States currentState;
    private float hunger;
    private float shyness;
    private float moveRate;
    private float xDest;
    private float zDest;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
    private NavMeshPath path;
    private int TableNbr;
    private Vector2[] Tables =
    {
        new Vector2(4.0f, -6.5f),
        new Vector2(4.0f, 6.9f),
        new Vector2(-4.0f, -6.5f),
        new Vector2(-4.0f, 6.9f),
        new Vector2(0.0f, 0.65f)
    };

    void Start()
    {
        TableNbr = 5;
        hasToWait = false;
        isAtTable = false;
        isSeekingForDistance = false;
        currentState = States.WAITING;
        timeToWait = 3.0f;
        obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hunger = Random.Range(0.0f, 100.0f);
        shyness = Random.Range(0.0f, 100.0f);
        moveRate = Random.Range(0.0f, 0.005f);
    }

    void LateUpdate()
    {
        if(hasToWait)
        {
            // In case that the agent must let an other agent pass the way before
            ManageWaiting();
        }
        else if(isSeekingForDistance)
        {
            MoveAway();
        }
        else if(currentState == States.WAITING)
        {
            ChooseActionWhileWaiting();
        }
        else if(currentState == States.EATING)
        {
            ChooseActionWhileEating();
        }
        else if(currentState == States.GOINGTOEAT)
        {
            CheckReachTable();
        }
        else if(currentState == States.GOINGAWAY)
        {
            CheckReachDest();
        }
    }

    void CheckReachTable()
    {
        if(isAtTable)
        {
            animator.SetBool("isWalking", false);
            animator.Rebind();
            animator.SetBool("isEating", true);
            agent.enabled = false;
            obstacle.enabled = true;
            currentState = States.EATING;
        }
        else if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
        {
            //cas ou il n'arrive pas à atteindre la table
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                animator.SetBool("isWalking", false);
                animator.Rebind();
                animator.SetBool("isWaiting", true);
                agent.enabled = false;
                obstacle.enabled = true;
                currentState = States.WAITING;
            }
        }
    }

    void CheckReachDest()
    {
        if (agent.enabled)
        {
            if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.SetBool("isWalking", false);
                    animator.Rebind();
                    animator.SetBool("isWaiting", true);
                    agent.enabled = false;
                    obstacle.enabled = true;
                    currentState = States.WAITING;
                }
            }
        }
    }

    void ChooseActionWhileWaiting()
    {
        float choice = Random.Range(0.0f, 1.0f);

        // we still waiting 99.5% of the update and depending on a moveRate 
        if (choice >= (0.995f + moveRate))
        {
            if (Random.Range(0.0f, 100f) < shyness)
            {
                isSeekingForDistance = true;
                MoveAway();
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
                    currentState = States.GOINGTOTALK;
                }
            }
        }
    }

    void ChooseActionWhileEating()
    {
        float choice = Random.Range(0.0f, 1.0f);

        // we still eating 99.5% of the update
        if (choice >= 0.995f)
        {
            isSeekingForDistance = true;
            MoveAway();
        }
    }

    void SelectTable()
    {
        int TableIndex = 0;
        animator.SetBool("isWaiting", false);
        animator.Rebind();
        animator.SetBool("isWalking", true);
        obstacle.enabled = false;
        agent.enabled = true;
        TableIndex = ClosestTableIndex();
        xDest = Tables[TableIndex].x;
        zDest = Tables[TableIndex].y;
        path = new NavMeshPath();
        agent.CalculatePath(new Vector3(xDest, this.gameObject.transform.position.y, zDest), path);
        agent.SetPath(path);
        currentState = States.GOINGTOEAT;
    }

    void ManageWaiting()
    {
        if (timeToWait <= 0.0f)
        {
            animator.SetBool("isWaiting", false);
            animator.Rebind();
            animator.SetBool("isWalking", true);
            obstacle.enabled = false;
            agent.enabled = true;
            path = new NavMeshPath();
            agent.CalculatePath(new Vector3(xDest, this.gameObject.transform.position.y, zDest), path);
            agent.SetPath(path);
            hasToWait = false;
        }
        else
        {
            timeToWait -= Time.deltaTime;
        }
    }

    void MoveAway()
    {
        float minDist = float.MaxValue;
        float tmp;

        xDest = Random.Range(-10.7f, 9.7f);
        zDest = Random.Range(-6.7f, 6.7f);

        foreach (GameObject ag in AllAgents.agents)
        { 
            tmp = Vector3.Distance(ag.transform.position, new Vector3(xDest, this.gameObject.transform.position.y, zDest));

            if (tmp < minDist)
                minDist = tmp;
        }

        if (minDist >= 2.0f)
        {
            animator.SetBool("isWaiting", false);
            animator.SetBool("isEating", false);
            animator.Rebind();
            animator.SetBool("isWalking", true);
            obstacle.enabled = false;
            agent.enabled = true;
            path = new NavMeshPath();
            agent.CalculatePath(new Vector3(xDest, this.gameObject.transform.position.y, zDest), path);
            agent.SetPath(path);
            currentState = States.GOINGAWAY;
            isSeekingForDistance = false;
        }
    }

    int ClosestTableIndex()
    {
        int Index = 0;
        float MinDistance = float.MaxValue;
        float tmp;
        for(int i = 0;i<TableNbr;i++)
        {
            tmp = Mathf.Sqrt(Mathf.Pow(this.gameObject.transform.position.x - Tables[i].x, 2) +
                          Mathf.Pow(this.gameObject.transform.position.z - Tables[i].y, 2));

            if (MinDistance > tmp)
            {
                MinDistance = tmp;
                Index = i;
            }
        }

        return Index;
    }
}
