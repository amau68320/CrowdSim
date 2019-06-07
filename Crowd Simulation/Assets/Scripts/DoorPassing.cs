using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorPassing : MonoBehaviour
{
    public static bool isEvacuation = false;
    private Text spawnedNbr;
    private GameObject persoBlocked;
    private float timeStuck;

    private void Start()
    {
        timeStuck = 4.0f;
        spawnedNbr = GameObject.Find("SpawnedAgentsNbr").GetComponent<Text>();
    }

    private void Update()
    {
        if(persoBlocked != null)
        {
            timeStuck -= Time.deltaTime;
        }

        if(timeStuck <= 0.0f)
        {
            AllAgents.agents.Remove(persoBlocked);
            Destroy(persoBlocked);
        }

        if(persoBlocked == null)
        {
            timeStuck = 4.0f;
        }
    }

    // at the enter we just count the number of agent detected by the trigger GameObject, at evacuation we do that as well but destroy the agent as well (it's outside, so it's safe)
    private void OnTriggerExit(Collider perso)
    {
        if ((string.Equals(perso.gameObject.name, "male(Clone)") || string.Equals(perso.gameObject.name, "female(Clone)")) && !isEvacuation)
            CheckAllEntered.nbrAgentEntered++;

        if ((string.Equals(perso.gameObject.name, "male(Clone)") || string.Equals(perso.gameObject.name, "female(Clone)")) && isEvacuation)
        {
            AllAgents.agents.Remove(perso.gameObject);
            Destroy(perso.gameObject);
            TriggerAlarm.nbrAgentsInRoom--;
            spawnedNbr.text = "Number of agents currently spawned : " + TriggerAlarm.nbrAgentsInRoom;
        }
    }

    //security if agent is stuck while a door closed (pretty rare) 
    private void OnTriggerEnter(Collider perso)
    {
        if ((string.Equals(perso.gameObject.name, "male(Clone)") || string.Equals(perso.gameObject.name, "female(Clone)")) && isEvacuation)
        {
            persoBlocked = perso.gameObject;
            timeStuck = 4.0f;
        }
    }
}
