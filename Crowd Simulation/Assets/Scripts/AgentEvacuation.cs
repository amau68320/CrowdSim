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
    private NavMeshAgent agent;
    private Animator animator;
    private NavMeshObstacle obstacle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();
        Destroy(this.gameObject.GetComponentInChildren<CollisionDetect>());
        obstacle.enabled = false;
        agent.enabled = true;
        xDest = Random.Range(0, 2) == 0 ? 16.0f : -16.0f;
        zDest = -1.27f;
        agent.ResetPath();
        agent.SetDestination(new Vector3(xDest, this.gameObject.transform.position.y, zDest));
        agent.speed = 2.5f;
        animator.SetBool("isTalking", false);
        animator.SetBool("isWaiting", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isEating", false);
        animator.Rebind();
        animator.SetBool("isRunning", true);
    }

    void Update()
    {
        
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
