using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAllEntered : MonoBehaviour
{
    GameObject trigger1;
    GameObject trigger2;
    float waitTime = 3f;
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
            GameObject.Find("doorP1-2").transform.Rotate(0, 150, 0, Space.Self);
            nbrAgentEntered = 0;
            waitTime = 3f;

            this.enabled = false;
        }
    }
}
