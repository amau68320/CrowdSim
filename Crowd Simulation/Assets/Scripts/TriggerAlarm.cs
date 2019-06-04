using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerAlarm : MonoBehaviour
{
    public delegate void AlarmTrigger();
    public static event AlarmTrigger onAlarm;

    public delegate void DoorClose(bool door);
    public static event DoorClose onClose;

    private Renderer[] alarms;
    private bool isRed;
    private bool isActivated;
    private bool anyDoorClosed;
    private float evacuateTime;
    private Text timeEvacuating;
    public const float blinkTime = 0.3f;
    public static int nbrAgentsInRoom = AgentSpawn.maxAgentNbr;

    void Start()
    {
        isRed = false;
        isActivated = false;
        anyDoorClosed = false;
        evacuateTime = 0.0f;
        alarms = GetComponentsInChildren<Renderer>();
        timeEvacuating = GameObject.Find("EvacuationTime").GetComponent<Text>();
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
            DoorPassing.isEvacuation = true;
        }

        if((nbrAgentsInRoom > 0) && isActivated)
        {
            evacuateTime += Time.deltaTime;
            timeEvacuating.text = "Time for evacuating the room : " + string.Format("{0:0.00}", evacuateTime) + " seconds";
        }


        if((Random.Range(0.0f,100.0f) > 99.95f) && !anyDoorClosed && isActivated)
        {
            anyDoorClosed = true;
            GameObject doorToClose;

            foreach (GameObject ag in AllAgents.agents)
            {
                onClose += ag.GetComponent<AgentEvacuation>().ReactToDoorClosed;
            }

            if (Random.Range(0, 2) == 0)
            {
                doorToClose = GameObject.Find("doorP1");
                onClose(true);
            }
            else
            {
                doorToClose = GameObject.Find("doorP1-2");
                onClose(false);
            }

            doorToClose.transform.Rotate(0, 150, 0, Space.Self);
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
