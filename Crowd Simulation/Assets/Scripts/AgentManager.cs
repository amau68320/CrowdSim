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
    GOINGAWAY = 5,
    WANTTOTALK = 6,
    WALKINGTOSOMEONE = 7
}

// the agent behavior during the party will be managed by a state machine
public class AgentManager : MonoBehaviour
{
    public delegate void TalkAction(GameObject talker);
    public static event TalkAction onTalk;
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
    private int TableIndex;
    public GameObject personToTalk;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
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
        TableIndex = 0;
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
        moveRate = Random.Range(0.0f, 0.001f);
        onTalk += ChooseToTalk;
    }

    void Update()
    {
        if (hasToWait)
        {
            // In case that the agent must let an other agent pass the way before
            ManageWaiting();
        }
        else if (isSeekingForDistance)
        {
            MoveAway();
        }
        else if (currentState == States.WAITING)
        {
            ChooseActionWhileWaiting();
        }
        else if (currentState == States.EATING)
        {
            ChooseActionWhileEating();
        }
        else if (currentState == States.GOINGTOEAT)
        {
            CheckReachTable();
        }
        else if (currentState == States.GOINGAWAY)
        {
            CheckReachDest();
        }
        else if (currentState == States.WANTTOTALK)
        {
            if (onTalk != null)
                onTalk(this.gameObject);
        }
        else if (currentState == States.WALKINGTOSOMEONE)
        {
            CheckReachPerson();
        }
    }

    void CheckReachTable()
    {
        if(isAtTable)
        {
            Vector3 relativePos = new Vector3(Tables[TableIndex].x, this.gameObject.transform.position.y, Tables[TableIndex].y) - this.gameObject.transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            this.gameObject.transform.rotation = rotation;
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

    void CheckReachPerson()
    {
        if (agent.enabled)
        {
            if (!agent.pathPending)
            {
                    if (Mathf.Sqrt(Mathf.Pow(this.gameObject.transform.position.x - personToTalk.transform.position.x
                    , 2.0f) + Mathf.Pow(this.gameObject.transform.position.z - personToTalk.transform.position.z, 2.0f)) <= 2.0f)
                    {
                        animator.SetBool("isWalking", false);
                        animator.Rebind();
                        animator.SetBool("isTalking", true);
                        agent.enabled = false;
                        obstacle.enabled = true;
                        currentState = States.TALKING;
                        Vector3 relativePos = personToTalk.transform.position - this.gameObject.transform.position;
                        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                        this.gameObject.transform.rotation = rotation;

                        if (!(personToTalk.GetComponent<AgentManager>().currentState == States.TALKING))
                        {
                            Vector3 rPos = this.gameObject.transform.position - personToTalk.transform.position;
                            Quaternion rot = Quaternion.LookRotation(rPos, Vector3.up);
                            personToTalk.transform.rotation = rot;
                            personToTalk.GetComponent<AgentManager>().animator.SetBool("isWaiting", false);
                            personToTalk.GetComponent<AgentManager>().animator.SetBool("isTalking", true);
                            personToTalk.GetComponent<AgentManager>().currentState = States.TALKING;
                        }
                    }
                    else
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

        // we still waiting 99.9% of the update and depending on a moveRate 
        if (choice >= (0.999f - moveRate))
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
                    foreach(GameObject ag in AllAgents.agents)
                    {
                        if (ag.GetComponent<AgentManager>().enabled)
                        {
                            if(ag.GetComponent<AgentManager>().currentState == States.WANTTOTALK)
                                currentState = States.GOINGTOTALK;
                        }
                    }
                    if (currentState != States.GOINGTOTALK)
                        currentState = States.WANTTOTALK;
                }
            }
        }
    }

    void ChooseActionWhileEating()
    {
        float choice = Random.Range(0.0f, 1.0f);

        // we still eating 99.8% of the update depending of the hunger as well
        if (choice >= (0.998f + hunger/100000.0f))
        {
            isSeekingForDistance = true;
            MoveAway();
        }
    }

    void ChooseActionWhileTalking()
    {
        float choice = Random.Range(0.0f, 1.0f);

        if(choice >= (0.998f - shyness/100000.0f))
        {
            isSeekingForDistance = true;
            MoveAway();
        }
    }

    void ChooseToTalk(GameObject talker)
    {
        Debug.Log("called");
        if(currentState != States.WANTTOTALK)
        {
            if(currentState == States.GOINGTOTALK)
            {
                obstacle.enabled = false;
                agent.enabled = true;
                agent.SetDestination(talker.transform.position);
                animator.SetBool("isWaiting", false);
                animator.Rebind();
                animator.SetBool("isWalking", true);
                xDest = talker.transform.position.x;
                zDest = talker.transform.position.z;
                personToTalk = talker;
                currentState = States.WALKINGTOSOMEONE;
            }
            else if(currentState == States.WAITING)
            {
                float choice = Random.Range(0.0f, 100.0f);
                float choice2 = Random.Range(0.0f, 100.0f);
                
                if((choice>99.99f) && (choice2>shyness))
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                    agent.SetDestination(talker.transform.position);
                    animator.SetBool("isWaiting", false);
                    animator.Rebind();
                    animator.SetBool("isWalking", true);
                    xDest = talker.transform.position.x;
                    zDest = talker.transform.position.z;
                    personToTalk = talker;
                    currentState = States.WALKINGTOSOMEONE;
                }
            }
            else if(currentState == States.EATING)
            {
                float choice = Random.Range(0.0f, 100.0f);
                float choice2 = Random.Range(0.0f, 100.0f);
                float choice3 = Random.Range(0.0f, 100.0f);

                if ((choice3 > 99.5f) && (choice > hunger) && (choice2 > shyness))
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                    agent.SetDestination(talker.transform.position);
                    animator.SetBool("isWaiting", false);
                    animator.Rebind();
                    animator.SetBool("isWalking", true);
                    xDest = talker.transform.position.x;
                    zDest = talker.transform.position.z;
                    personToTalk = talker;
                    currentState = States.WALKINGTOSOMEONE;
                }
            }
        }
    }

    void SelectTable()
    {
        obstacle.enabled = false;
        agent.enabled = true;
        TableIndex = ClosestTableIndex();
        xDest = Tables[TableIndex].x;
        zDest = Tables[TableIndex].y;
        agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
        currentState = States.GOINGTOEAT;
        animator.SetBool("isWaiting", false);
        animator.Rebind();
        animator.SetBool("isWalking", true);
    }

    void ManageWaiting()
    {
        if (timeToWait <= 0.0f)
        {
            obstacle.enabled = false;
            agent.enabled = true;
            agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
            hasToWait = false;
            animator.SetBool("isWaiting", false);
            animator.Rebind();
            animator.SetBool("isWalking", true);
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
            tmp = Mathf.Sqrt(Mathf.Pow(ag.transform.position.x - xDest, 2.0f) + Mathf.Pow(ag.transform.position.z - zDest, 2.0f));

            if (tmp < minDist)
                minDist = tmp;
        }

        if (minDist >= 1.5f)
        {
            isSeekingForDistance = false;
            obstacle.enabled = false;
            agent.enabled = true;
            agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
            currentState = States.GOINGAWAY;
            animator.SetBool("isWaiting", false);
            animator.SetBool("isEating", false);
            animator.Rebind();
            animator.SetBool("isWalking", true);
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
