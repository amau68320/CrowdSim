using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAlarm : MonoBehaviour
{
    public delegate void AlarmTrigger();
    public static event AlarmTrigger onAlarm;

    private Renderer[] alarms;
    private bool isRed;
    private bool isActivated;
    public const float blinkTime = 0.3f;

    void Start()
    {
        isRed = false;
        isActivated = false;
        alarms = GetComponentsInChildren<Renderer>();
        onAlarm += TriggerBlink;
        onAlarm += CheckAllEntered.OpenDoors;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !isActivated)
        {
            if (AllAgents.agents.Count != AgentSpawn.maxAgentNbr)
            {
                return;
            }
            foreach (GameObject ag in AllAgents.agents)
            {
                if (!ag.GetComponent<AgentManager>().enabled)
                    return;
            }
            foreach (GameObject ag in AllAgents.agents)
            {
                onAlarm += ag.GetComponent<AgentManager>().ReactToAlarm;
            }
            isActivated = true;
            onAlarm();
        }
    }

    void TriggerBlink()
    {
        InvokeRepeating("BlinkLamps", 0.0f, blinkTime);
    }

    void BlinkLamps()
    {
        if (isRed)
        {
            foreach (Renderer rend in alarms)
            {
                rend.material.SetColor("_EmissionColor", Color.white);
            }
        }
        else
        {
            foreach (Renderer rend in alarms)
            {
                rend.material.SetColor("_EmissionColor", Color.red);
            }
        }
        isRed = !isRed;
    }
}
