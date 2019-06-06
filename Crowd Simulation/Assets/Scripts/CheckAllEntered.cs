using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script checks if every agent is entered inside the Room and after that it waits 5 seconds to close the doors 
public class CheckAllEntered : MonoBehaviour
{
    GameObject trigger1;
    GameObject trigger2;
    float waitTime = 5.0f;
    int maxAgents = AgentSpawn.maxAgentNbr;
    public static int nbrAgentEntered = 0;

    void Update()
    {
        if(nbrAgentEntered>=maxAgents)
        {
            waitTime -= Time.deltaTime;
        }

        if(waitTime<=0)
        {
            GameObject.Find("doorP1").transform.Rotate(0, 150, 0, Space.Self);
            GameObject.Find("doorP2").transform.Rotate(0, 150, 0, Space.Self);
            nbrAgentEntered = 0;
            waitTime = 5.0f;

            this.enabled = false;
        }
    }

    // Called for evacuation
    public static void OpenDoors()
    {
        GameObject.Find("doorP1").transform.Rotate(0, -150, 0, Space.Self);
        GameObject.Find("doorP2").transform.Rotate(0, -150, 0, Space.Self);
    }
}
