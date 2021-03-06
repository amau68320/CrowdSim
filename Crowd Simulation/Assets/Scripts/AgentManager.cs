﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// different states during reception
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

// the agent behavior during the reception will be managed by a state machine
public class AgentManager : MonoBehaviour
{
    public delegate void TalkAction(GameObject talker);
    public static event TalkAction onTalk;

    private  bool hasToWait;
    private  bool isAtTable;
    private bool isSeekingForDistance;
    private float timeToWait;
    private States currentState;
    private float hunger;
    private float shyness;
    private float moveRate;
    private float xDest;
    private float zDest;
    private int TableIndex;
    private int TempoEvent;
    private GameObject personToTalk;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
    private int TableNbr;
    private List<GameObject> talkers;
    public static Vector2[] Tables =
    {
        new Vector2(4.0f, -6.5f),
        new Vector2(4.0f, 6.9f),
        new Vector2(-4.0f, -6.5f),
        new Vector2(-4.0f, 6.9f),
        new Vector2(0.0f, 0.65f)
    };

    private void Start()
    {
        TableNbr = 5;
        TableIndex = 0;
        TempoEvent = 50; 
        hasToWait = false;
        isAtTable = false;
        isSeekingForDistance = false;
        currentState = States.WAITING;
        timeToWait = 3.0f; // the time to wait after a "colision" is detected by the security triggered GameObject
        talkers = new List<GameObject>();
        obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hunger = Random.Range(0.0f, 100.0f); // define the hunger
        shyness = Random.Range(0.0f, 100.0f); // define the shyness (100 : very shy, 0 : sociable)
        moveRate = Random.Range(0.0f, 0.001f); // define the probability that the agent will move often 
        onTalk += ChooseToTalk;
    }

    private void Update()
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
        else if (currentState == States.TALKING)
        {
            ChooseActionWhileTalking();
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
            // We just call the event once per second (almost 50 Frames per second) 
            if (onTalk != null && (TempoEvent >= 50))
            {
                onTalk(this.gameObject);
                TempoEvent = 0;
            }
            TempoEvent++;
        }
        else if (currentState == States.WALKINGTOSOMEONE)
        {
            CheckReachPerson();
        }
    }

    private void CheckReachTable()
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

    private void CheckReachDest()
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

    private void CheckReachPerson()
    {
        if (agent.enabled)
        {
            if (!agent.pathPending)
            {
                if (AgentEvacuation.CalculateDistanceSquare(gameObject.transform.position, personToTalk.transform.position) <= 1.0f)
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

                    // if this agent is the first to reach the person who wants to talk, it triggers the state and animation 
                     if (personToTalk.GetComponent<AgentManager>().currentState != States.TALKING)
                     {
                         Vector3 rPos = this.gameObject.transform.position - personToTalk.transform.position;
                         Quaternion rot = Quaternion.LookRotation(rPos, Vector3.up);
                         personToTalk.transform.rotation = rot;
                         personToTalk.GetComponent<AgentManager>().animator.SetBool("isWaiting", false);
                         personToTalk.GetComponent<AgentManager>().animator.Rebind();
                         personToTalk.GetComponent<AgentManager>().animator.SetBool("isTalking", true);
                         personToTalk.GetComponent<AgentManager>().currentState = States.TALKING;
                         foreach (GameObject ag in personToTalk.GetComponent<AgentManager>().talkers)
                         {
                             ag.GetComponent<AgentManager>().talkers.Add(personToTalk);
                             foreach (GameObject ag2 in personToTalk.GetComponent<AgentManager>().talkers)
                             {
                                 if (ag != ag2)
                                    ag.GetComponent<AgentManager>().talkers.Add(ag2);
                             }
                         }
                     }
                }
            }
        }

        if((personToTalk.GetComponent<AgentManager>().currentState != States.TALKING) && (personToTalk.GetComponent<AgentManager>().currentState != States.WANTTOTALK))
        {
            foreach (GameObject ag in talkers)
            {
                ag.GetComponent<AgentManager>().talkers.Remove(this.gameObject);
            }
            talkers.Clear();
            animator.SetBool("isWalking", false);
            animator.Rebind();
            animator.SetBool("isWaiting", true);
            agent.enabled = false;
            obstacle.enabled = true;
            currentState = States.WAITING;
        }
    }

    private void ChooseActionWhileWaiting()
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
                else if(Random.Range(0.0f, 100.0f) > shyness)
                {
                    // call someone to talk
                    foreach(GameObject ag in AllAgents.agents)
                    {
                        if (ag.GetComponent<AgentManager>().enabled && (ag != this.gameObject))
                        {
                            if(ag.GetComponent<AgentManager>().currentState == States.WANTTOTALK)
                                currentState = States.GOINGTOTALK;
                        }
                    }
                    if (currentState != States.GOINGTOTALK)
                    {
                        currentState = States.WANTTOTALK;
                    }
                }
            }
        }
    }

    private void ChooseActionWhileTalking()
    {
        if(talkers.Count == 0)
        {
            animator.SetBool("isTalking", false);
            animator.Rebind();
            animator.SetBool("isWaiting", true);
            currentState = States.WAITING;
        }

        float choice = Random.Range(0.0f, 1.0f);

        // we still waiting 99.9% of the update and depending on a moveRate 
        if (choice >= (0.999f - moveRate))
        {
            if (Random.Range(0.0f, 100f) < shyness)
            {
                isSeekingForDistance = true;

                foreach (GameObject ag in talkers)
                {
                    ag.GetComponent<AgentManager>().talkers.Remove(this.gameObject);
                }
                talkers.Clear();

                MoveAway();
            }
            else if (Random.Range(0.0f, 100.0f) < hunger)
            {
                foreach (GameObject ag in talkers)
                {
                    ag.GetComponent<AgentManager>().talkers.Remove(this.gameObject);
                }
                talkers.Clear();

                SelectTable();
            }
        }
    }

    private void ChooseActionWhileEating()
    {
        float choice = Random.Range(0.0f, 1.0f);

        // we still eating 99.85% of the update depending of the hunger as well
        if (choice >= (0.9985f + hunger/100000.0f))
        {
            isSeekingForDistance = true;
            MoveAway();
        }
    }

    // this method is called when event "onTalk" is catched 
    private void ChooseToTalk(GameObject talker)
    {
        // the conversation can be with max 4 other peoples 
        // the choice to talk depends on the current state and on shyness and a random variable choice inside the method 
        if((currentState != States.WANTTOTALK) && (talker.GetComponent<AgentManager>().talkers.Count < 4))
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
                talker.GetComponent<AgentManager>().talkers.Add(this.gameObject);
            }
            else if(currentState == States.WAITING)
            {
                float choice = Random.Range(0.0f, 100.0f);
                float choice2 = Random.Range(0.0f, 100.0f);
                
                if((choice>95.0f) && (choice2>shyness))
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
                    talker.GetComponent<AgentManager>().talkers.Add(this.gameObject);
                }
            }
            else if(currentState == States.EATING)
            {
                float choice = Random.Range(0.0f, 100.0f);
                float choice2 = Random.Range(0.0f, 100.0f);
                float choice3 = Random.Range(0.0f, 100.0f);

                if ((choice3 > 85.0f) && (choice > hunger) && (choice2 > shyness))
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                    agent.SetDestination(talker.transform.position);
                    animator.SetBool("isEating", false);
                    animator.Rebind();
                    animator.SetBool("isWalking", true);
                    xDest = talker.transform.position.x;
                    zDest = talker.transform.position.z;
                    personToTalk = talker;
                    currentState = States.WALKINGTOSOMEONE;
                    talker.GetComponent<AgentManager>().talkers.Add(this.gameObject);
                }
            }
        }
    }

    // When agent wants to eat, select a table and set it as destination
    private void SelectTable()
    {
        obstacle.enabled = false;
        agent.enabled = true;
        TableIndex = ClosestTableIndex();
        xDest = Tables[TableIndex].x;
        zDest = Tables[TableIndex].y;
        agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
        currentState = States.GOINGTOEAT;
        animator.SetBool("isWaiting", false);
        animator.SetBool("isTalking", false);
        animator.Rebind();
        animator.SetBool("isWalking", true);
    }

    // if agent has to wait because of a collision is detected by security trigger 
    private void ManageWaiting()
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

    private void MoveAway()
    {
        float minDist = float.MaxValue;
        float tmp;

        xDest = Random.Range(-10.7f, 9.7f);
        zDest = Random.Range(-6.7f, 6.7f);

        foreach (GameObject ag in AllAgents.agents)
        {
            tmp = AgentEvacuation.CalculateDistanceSquare(ag.transform.position, new Vector3(xDest, 0.0f, zDest));

            if (tmp < minDist)
                minDist = tmp;
        }

        //distance must be greater than 1.5f
        if (minDist >= 2.25f)
        {
            isSeekingForDistance = false;
            obstacle.enabled = false;
            agent.enabled = true;
            agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
            currentState = States.GOINGAWAY;
            animator.SetBool("isWaiting", false);
            animator.SetBool("isEating", false);
            animator.SetBool("isTalking", false);
            animator.Rebind();
            animator.SetBool("isWalking", true);
        }
    }

    // Choose the closest table
    private int ClosestTableIndex()
    {
        int Index = 0;
        float MinDistance = float.MaxValue;
        float tmp;
        for(int i = 0;i<TableNbr;i++)
        {
            tmp = AgentEvacuation.CalculateDistanceSquare(gameObject.transform.position, new Vector3(Tables[i].x, 0.0f, Tables[i].y));

            if (MinDistance > tmp)
            {
                MinDistance = tmp;
                Index = i;
            }
        }

        return Index;
    }

    // declenched by catching event "onAlarm", turn off this script and turn on the AgentEvacuation script to manage evacuation
    // give some useful variables to AgentEvacuation script as well
    public void ReactToAlarm()
    {
        AgentEvacuation nextScript = GetComponent<AgentEvacuation>();
        nextScript.enabled = true;
        nextScript.SetCurrentState(currentState);
        nextScript.SetHunger(hunger);
        nextScript.SetShyness(shyness);
        nextScript.SetMoveRate(moveRate);
        this.enabled = false;
    }

    public void SetIsAtTable(bool atTable)
    {
        isAtTable = atTable;
    }

    public void SetHasToWait(bool toWait)
    {
        hasToWait = toWait;
    }

    public static void ResetOnTalk()
    {
        onTalk = null;
    }
}
