using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtTableCheck : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponentInParent<AgentManager>().enabled)
            other.gameObject.GetComponentInParent<AgentManager>().isAtTable = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<AgentManager>().enabled)
            other.gameObject.GetComponentInParent<AgentManager>().isAtTable = false;
    }
}
