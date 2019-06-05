using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private GameObject personHelped;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
    private int nbrDoorOpened;

    public Vector2[] EvacuationDest =
    {
        new Vector2(14.0f, -1.27f),
        new Vector2(-13.5f, -0.2f)
    };

    void Start()
    {
        decontracteness = Random.Range(0.0f, 100.0f);
        helpness = Random.Range(0.0f, 100.0f);
        nbrDoorOpened = 2;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();
        Destroy(this.gameObject.GetComponentInChildren<CollisionDetect>());
        hasThought = false;
        hasStartedThinking = false;
        isInPanic = false;
        isHelped = false;
        isGoingToHelp = false;

        DeterminateTimeReacting();
    }

    void Update()
    {
        if (timeEating <= 0.0f)
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

            if ((timeThinking <= 0) && !hasThought)
            {
                StartEvacuating();
            }
        }
        else
        {
            timeEating -= Time.deltaTime;
        }

        if(isInPanic)
        {
            timeBeforeASkingHelp -= Time.deltaTime;

            if(CheckAndChangePanicDest())
            {
                foreach(GameObject ag in AllAgents.agents)
                {
                    //security to avoid someone to leave the room before deciding to help (or not) someone
                    if (Mathf.Sqrt(Mathf.Pow(ag.transform.position.x - ag.GetComponent<NavMeshAgent>().destination.x
                        , 2.0f) + Mathf.Pow(ag.transform.position.z - ag.GetComponent<NavMeshAgent>().destination.z, 2.0f)) >= 3.0f)
                    {
                        onAskHelp += ag.GetComponent<AgentEvacuation>().ChooseToHelp;
                    }
                }

                onAskHelp(this.gameObject);
                isInPanic = false;
            }
        }

        if(isGoingToHelp)
        {
            CheckReachPersonToHelp();
        }
    }

    int ClosestDoorIndex()
    {
        int Index = 0;
        float MinDistance = float.MaxValue;
        float tmp;
        for (int i = 0; i < nbrDoorOpened; i++)
        {
            tmp = Mathf.Sqrt(Mathf.Pow(this.gameObject.transform.position.x - EvacuationDest[i].x, 2) +
                          Mathf.Pow(this.gameObject.transform.position.z - EvacuationDest[i].y, 2));

            if (MinDistance > tmp)
            {
                MinDistance = tmp;
                Index = i;
            }
        }

        return Index;
    }

    void StartEvacuating()
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
            agent.speed = 2.5f - (hunger / 200.0f) - (shyness / 200.0f) + (moveRate * 1000.0f);
        else
            agent.speed = 1.2f;

        hasThought = true;
    }

    void DeterminateTimeReacting()
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

    bool CheckAndChangePanicDest()
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
                        obstacle.enabled = false;
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

    void CheckReachPersonToHelp()
    {
        if (agent.enabled && !agent.pathPending)
        {
            if (Mathf.Sqrt(Mathf.Pow(this.gameObject.transform.position.x - xDest
                , 2.0f) + Mathf.Pow(this.gameObject.transform.position.z - zDest, 2.0f)) <= 1.0f)
            {
                AgentEvacuation script = personHelped.GetComponent<AgentEvacuation>();
                int index = ClosestDoorIndex();
                xDest = EvacuationDest[index].x;
                zDest = EvacuationDest[index].y;
                script.xDest = xDest;
                script.zDest = zDest;
                script.animator.SetBool("isInPanic", false);
                script.animator.Rebind();
                script.animator.SetBool("isRunning", true);
                script.obstacle.enabled = false;
                script.agent.enabled = true;
                agent.ResetPath();
                agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
                script.agent.SetDestination(new Vector3(script.xDest, script.gameObject.transform.position.y, script.zDest));
                isGoingToHelp = false;
            }
        }
    }

    public void ReactToDoorClosed(bool whichDoor)
    {
        nbrDoorOpened--;

        float destX = whichDoor ? EvacuationDest[0].x : EvacuationDest[1].x;

        if (whichDoor)
        {
            EvacuationDest[0].x = EvacuationDest[1].x;
            EvacuationDest[0].y = EvacuationDest[1].y;
        }

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

    public void ChooseToHelp(GameObject personToHelp)
    {
        if (this == null || personToHelp.GetComponent<AgentEvacuation>().isHelped || isGoingToHelp)
            return;

        float distance = Mathf.Sqrt(Mathf.Pow(gameObject.transform.position.x - personToHelp.transform.position.x
                         , 2.0f) + Mathf.Pow(gameObject.transform.position.z - personToHelp.transform.position.z, 2.0f));

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
}
