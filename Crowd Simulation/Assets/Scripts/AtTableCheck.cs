using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script realise the check to say if an Agent is at table during the reception (in purpose of triggering the eating animation)
public class AtTableCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponentInParent<AgentManager>().enabled)
            other.gameObject.GetComponentInParent<AgentManager>().SetIsAtTable(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<AgentManager>().enabled)
            other.gameObject.GetComponentInParent<AgentManager>().SetIsAtTable(false);
    }
}
