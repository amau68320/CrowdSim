using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This script manage the Evacuation for each agent
public class AgentEvacuation : MonoBehaviour
{
    public delegate void AskHelp(GameObject personToHelp);
    public static event AskHelp onAskHelp;

    private States currentState;
    private float hunger;
    private float shyness;
    private float moveRate;
    private float decontracteness;
    private float helpness;
    private float xDest;
    private float zDest;
    private float timeEating;
    private float timeThinking;
    private float timeBeforeASkingHelp;
    private bool hasThought;
    private bool hasStartedThinking;
    private bool isInPanic;
    private bool isHelped;
    private bool isGoingToHelp;
    private bool hasCalledHelp;
    private GameObject personHelped;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
    private int nbrDoorOpened;

    // The two "outside" possible destinations, if you add other doors for leaving you'll have to add coordinates here 
    public Vector2[] EvacuationDest =
    {
        new Vector2(14.0f, -1.27f),
        new Vector2(-13.5f, -0.2f)
    };

    private void Start()
    {
        decontracteness = Random.Range(0.0f, 100.0f); // decontractness determines the level of panic (close to 100 : no panic, close to 0 : panic a lot)
        helpness = Random.Range(0.0f, 100.0f);// helpness determines the probability that this agent helps an other in trouble
        nbrDoorOpened = 2;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();
        Destroy(this.gameObject.GetComponentInChildren<CollisionDetect>()); // we don't have to check if agents are blocking each other during the evacuation because they all move
        hasThought = false;
        hasStartedThinking = false;
        isInPanic = false;
        isHelped = false;
        isGoingToHelp = false;
        hasCalledHelp = false;

        DeterminateTimeReacting();
    }

    private void Update()
    {
        if (timeEating <= 0.0f) // agents that are eating can continue eat some seconds after the alarm was triggered
        {
            if (!hasStartedThinking)
            {
                animator.SetBool("isTalking", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isEating", false);
                animator.Rebind();
                animator.SetBool("isWaiting", true);
                agent.enabled = false;
                obstacle.enabled = true;
                hasStartedThinking = true;
            }

            timeThinking -= Time.deltaTime;

            if ((timeThinking <= 0) && !hasThought) // the agents have "a bug moment" after the alarm is triggered (the reaction time) 
            {
                StartEvacuating();
            }
        }
        else
        {
            timeEating -= Time.deltaTime;
        }

        if (isInPanic && !hasCalledHelp) // if the agent is in panic situation
        {
            timeBeforeASkingHelp -= Time.deltaTime;

            if (CheckAndChangePanicDest())
            {
                hasCalledHelp = true;
                foreach (GameObject ag in AllAgents.agents)
                {
                    //security to avoid someone to leave the room before deciding to help (or not) someone, the real distance is  3.0f because we use square distance
                    if (CalculateDistanceSquare(ag.transform.position, ag.GetComponent<NavMeshAgent>().destination) >= 9.0f)
                    {
                        onAskHelp += ag.GetComponent<AgentEvacuation>().ChooseToHelp;
                    }
                }

                onAskHelp(this.gameObject); // send an event to every agents (that are not too close of the door)
            }
        }

        if(isGoingToHelp) // if the agent decide to help an other one in trouble
        {
            CheckReachPersonToHelp();
        }
    }

    // return the closest door index near the agent (basically return 0 if a door is closed) 
    private int ClosestDoorIndex()
    {
        int Index = 0;
        float MinDistance = float.MaxValue;
        float tmp;
        for (int i = 0; i < nbrDoorOpened; i++)
        {
            tmp = CalculateDistanceSquare(gameObject.transform.position, new Vector3(EvacuationDest[i].x, 0.0f, EvacuationDest[i].y)); 

            if (MinDistance > tmp)
            {
                MinDistance = tmp;
                Index = i;
            }
        }

        return Index;
    }

    // choose to run or walk depending on decontractness and choose if agent is in panic or not (in panic agent can't reach a door) 
    private void StartEvacuating()
    {
        if (decontracteness <= 2.0f)
        {
            xDest = Random.Range(-10.7f, 9.7f);
            zDest = Random.Range(-6.7f, 6.7f);
            isInPanic = true;
            timeBeforeASkingHelp = Random.Range(2.0f, 6.0f) + (shyness / 20.0f);
        }
        else
        {
            int index = ClosestDoorIndex();
            xDest = EvacuationDest[index].x;
            zDest = EvacuationDest[index].y;
        }

        animator.SetBool("isWaiting", false);
        animator.Rebind();

        if (decontracteness >= 75.0f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }

        obstacle.enabled = false;
        agent.enabled = true;
        agent.ResetPath();
        agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));

        if (decontracteness < 75.0f)
            agent.speed = 2.5f - (hunger / 200.0f) - (shyness / 200.0f) + (moveRate * 1000.0f); // speed depends on shyness, hunger and moveRate
        else
            agent.speed = 1.2f; // same speed as enter for walking agents

        hasThought = true;
    }

    // The time reaction depends on the state during reception
    private void DeterminateTimeReacting()
    {
        timeThinking = Random.Range(3.0f, 8.0f);
        timeEating = 0.0f;

        if (currentState == States.EATING)
        {
            timeEating = Random.Range(0.5f, 3.5f) + (hunger / 50.0f);

        }
        else if (currentState == States.TALKING)
        {
            timeThinking = Random.Range(5.0f, 10.0f);
        }

        if (decontracteness >= 70.0f)
        {
            if (currentState == States.TALKING)
            {
                timeThinking = Random.Range(7.0f, 12.0f);
            }
            else
            {
                timeThinking = Random.Range(4.5f, 9.0f);
            }
        }
    }

    // Agents that are in panic have a time to stop running and ask help, while this time is not under 0 agents still runnings to random destination 
    private bool CheckAndChangePanicDest()
    {
        if (agent.enabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    if (timeBeforeASkingHelp <= 0.0f)
                    {
                        animator.SetBool("isRunning", false);
                        animator.Rebind();
                        animator.SetBool("isInPanic", true);
                        agent.enabled = false;
                        obstacle.enabled = true;
                        return true;
                    }
                    else
                    {
                        xDest = Random.Range(-10.7f, 9.7f);
                        zDest = Random.Range(-6.7f, 6.7f);
                        agent.ResetPath();
                        agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
                    }
                }
            }
        }

        return false;
    }

    // When an agent wants to help another, it has to check if it reachs this other agent
    private void CheckReachPersonToHelp()
    {
        if (agent.enabled && !agent.pathPending)
        {
            // we check if the agent is near of the agent to help but not at the exact position (we don't want a colision) 
            if (CalculateDistanceSquare(gameObject.transform.position, new Vector3(xDest, 0.0f, zDest)) <= 1.0f)
            {
                int index = ClosestDoorIndex();
                xDest = EvacuationDest[index].x;
                zDest = EvacuationDest[index].y;
                agent.ResetPath();
                agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
                isGoingToHelp = false;

                // security but should never happend 
                if (personHelped != null)
                { 
                    AgentEvacuation script = personHelped.GetComponent<AgentEvacuation>();
                    script.hasCalledHelp = false;
                    script.xDest = xDest;
                    script.zDest = zDest;
                    script.animator.SetBool("isInPanic", false);
                    script.animator.Rebind();
                    script.animator.SetBool("isRunning", true);
                    script.obstacle.enabled = false;
                    script.agent.enabled = true;
                    script.agent.SetDestination(new Vector3(script.xDest, script.gameObject.transform.position.y, script.zDest));
                }
            }
        }
    }

    // reaction to the event "onCloseDoor"
    public void ReactToDoorClosed(bool whichDoor)
    {
        // we simply say : there is only one door opened and it's at the index 0 
        nbrDoorOpened--;

        float destX = whichDoor ? EvacuationDest[0].x : EvacuationDest[1].x; 

        if (whichDoor)
        {
            EvacuationDest[0].x = EvacuationDest[1].x;
            EvacuationDest[0].y = EvacuationDest[1].y;
        }

        // the agents who were going to the closed door have a "bug time", they think about what they should do and after this time they decide to go to the other door (or panic)
        if (agent.enabled && (agent.destination.x == destX)) 
        {
            agent.enabled = false;
            obstacle.enabled = true;
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.Rebind();
            animator.SetBool("isWaiting", true);
            hasThought = false;
            timeThinking = Random.Range(1.0f, 4.0f);
        }

        decontracteness -= 15.0f;
    }

    // react to the event "OnASkHelp"
    public void ChooseToHelp(GameObject personToHelp)
    {
        if (this == null || personToHelp.GetComponent<AgentEvacuation>().isHelped || isGoingToHelp)
            return;

        // we take Sqrt in this case because we want the real position
        float distance = Mathf.Sqrt(CalculateDistanceSquare(gameObject.transform.position, personToHelp.transform.position)); 

        // help an agent depend on distance and helpness, the check of "isHelped" is just a security 
        if(((helpness - distance) >= 50.0f) && !personToHelp.GetComponent<AgentEvacuation>().isHelped) 
        {
            personToHelp.GetComponent<AgentEvacuation>().isHelped = true;
            isGoingToHelp = true;
            personHelped = personToHelp;
            obstacle.enabled = false;
            agent.enabled = true;
            animator.SetBool("isWaiting", false);
            animator.SetBool("isWalking", false);
            animator.Rebind();
            animator.SetBool("isRunning", true);
            xDest = personToHelp.transform.position.x;
            zDest = personToHelp.transform.position.z;
            agent.speed = 2.5f - (hunger / 200.0f) - (shyness / 200.0f) + (moveRate * 1000.0f);
            agent.ResetPath();
            agent.SetDestination(new Vector3(xDest, personToHelp.transform.position.y, zDest));
        }
    }

    public void SetCurrentState(States state)
    {
        currentState = state;
    }

    public void SetHunger(float hung)
    {
        hunger = hung;
    }

    public void SetShyness(float shy)
    {
        shyness = shy;
    }

    public void SetMoveRate(float move)
    {
        moveRate = move;
    }

    public bool HasCalledHelp()
    {
        return hasCalledHelp;
    }

    public static void ResetOnAskHelp()
    {
        onAskHelp = null;
    }

    // calculate square distance between 2 Vector3 (more optimised than doing the sqrt that gives the real distance), we only need to calculate 2D distance here
    // because agents evolve on a 2D grid (on X and Z axes) 
    public static float CalculateDistanceSquare(Vector3 a, Vector3 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z);
    }
}
