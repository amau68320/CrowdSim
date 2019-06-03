using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentEvacuation : MonoBehaviour
{
    public States currentState;
    public float hunger;
    public float shyness;
    public float moveRate;
    private float decontracteness;
    private float xDest;
    private float zDest;
    private float timeEating;
    private float timeThinking;
    private bool hasThought;
    private bool hasStartedThinking;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;
    private int nbrDoorOpened;
    public static Vector2[] EvacuationDest =
    {
        new Vector2(16.0f, -1.27f),
        new Vector2(-16.0f, -1.27f)
    };

    void Start()
    {
        decontracteness = Random.Range(0.0f, 100.0f);
        nbrDoorOpened = 2;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();
        Destroy(this.gameObject.GetComponentInChildren<CollisionDetect>());
        timeThinking = Random.Range(3.0f, 8.0f);
        hasThought = false;
        hasStartedThinking = false;

        if (currentState == States.EATING)
        {
            timeEating = Random.Range(0.5f, 3.5f) + (hunger / 50.0f);

        }
        else if(currentState == States.TALKING)
        {
                timeThinking = Random.Range(5.0f, 10.0f);
        }
        else
        {
            timeEating = 0.0f;
        }

        if(decontracteness >= 70.0f)
        {
            if(currentState == States.TALKING)
            {
                timeThinking = Random.Range(7.0f, 12.0f);
            }
            else
            {
                timeThinking = Random.Range(4.5f, 9.0f);
            }
        }

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
                int index = ClosestDoorIndex();
                xDest = EvacuationDest[index].x;
                zDest = EvacuationDest[index].y;
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

                hasThought = true;
            }
        }
        else
        {
            timeEating -= Time.deltaTime;
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

    public void reactToDoorClosed(bool whichDoor)
    {
        if(whichDoor)
            EvacuationDest[0] = new Vector2(-16.0f, -1.7f);

        nbrDoorOpened--;

        float destX = whichDoor ? 16.0f : -16.0f;

        if(agent.enabled && (agent.destination.x == destX))
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

        decontracteness -= 20.0f;
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
