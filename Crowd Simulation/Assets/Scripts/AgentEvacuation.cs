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
    private float xDest;
    private float zDest;
    private float timeThinking;
    private bool hasThounght;
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;

    void Start()
    {
        timeThinking = Random.Range(1.5f, 7.0f);
        hasThounght = false;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();
        Destroy(this.gameObject.GetComponentInChildren<CollisionDetect>());
        animator.SetBool("isTalking", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isEating", false);
        animator.Rebind();
        animator.SetBool("isWaiting", true);
        agent.enabled = false;
        obstacle.enabled = true;
    }

    void Update()
    {
        timeThinking -= Time.deltaTime;

        if((timeThinking <= 0) && !hasThounght)
        {
            animator.SetBool("isWaiting", false);
            animator.Rebind();
            animator.SetBool("isRunning", true);
            obstacle.enabled = false;
            agent.enabled = true;
            xDest = Random.Range(0, 2) == 0 ? 16.0f : -16.0f;
            zDest = -1.27f;
            agent.ResetPath();
            agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
            agent.speed = 2.5f;
            hasThounght = true;
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
