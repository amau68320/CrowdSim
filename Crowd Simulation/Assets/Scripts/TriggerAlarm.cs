using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriggerAlarm : MonoBehaviour
{
    public delegate void AlarmTrigger();
    public static event AlarmTrigger onAlarm;

    public delegate void DoorClose(bool door);
    public static event DoorClose onClose;

    public static string endText;
    public static int nbrAgentsInRoom;

    public const float blinkTime = 0.3f;


    private Renderer[] alarms;
    private bool isRed;
    private bool isActivated;
    private bool anyDoorClosed;
    private float evacuateTime;
    private Text timeEvacuating;

    private void Start()
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

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !isActivated)
        {
            ActivateAlarm();
        }

        if((nbrAgentsInRoom > 0) && isActivated)
        {
            evacuateTime += Time.deltaTime; //update the evacuation time text
            timeEvacuating.text = "Time for evacuating the room : " + string.Format("{0:0.00}", evacuateTime) + " seconds";
        }


        if((Random.Range(0.0f,100.0f) > 99.95f) && !anyDoorClosed && isActivated)
        {
            CloseADoor();
        }

        if(isActivated)
            CheckEvacuationOver();
    }

    private void TriggerBlink()
    {
        InvokeRepeating("BlinkLamps", 0.0f, blinkTime); // blink the lamps in red all "blinkTime" seconds
    }

    private void BlinkLamps()
    {
        // We update the Shader to change color of each lamp
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

    private void CheckEvacuationOver()
    {
        // We verify if all agents left the room or all "not in panic" agents left 
        if (AllAgents.agents.Count == 0)
        {
            endText = "The evacuation went well and took : " + string.Format("{0:0.00}", evacuateTime) + " seconds ";
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            foreach (GameObject ag in AllAgents.agents)
            {
                if (!ag.GetComponent<AgentEvacuation>().HasCalledHelp())
                    return;
            }

            endText = "The evacuation didn't go well, there is/are : " + AllAgents.agents.Count + " persons that didn't escape";
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void ActivateAlarm()
    {
        // We check if all agents are spawned and if they all have AgentManager script activate (they are in reception mode)
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
        onAlarm(); // Send event to all agents
        DoorPassing.isEvacuation = true;
    }

    private void CloseADoor()
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
            onClose(true); // Send an event to all agents
        }
        else
        {
            doorToClose = GameObject.Find("doorP2");
            onClose(false); // send an event to all agents
        }

        doorToClose.transform.Rotate(0, 150, 0, Space.Self); // close the selected door
    }

    public static void ResetOnAlarm()
    {
        onAlarm = null;
    }

    public static void ResetOnClose()
    {
        onClose = null;
    }
}
