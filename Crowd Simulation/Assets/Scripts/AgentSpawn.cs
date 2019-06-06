using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentSpawn : MonoBehaviour
{
    public GameObject male;
    public GameObject female;
    public const float spawnTime = 0.3f;
    public static int maxAgentNbr = 80;
    private int nbrSpawnedAgents = 0;
    private GameObject spawn1;
    private GameObject spawn2;
    private Text spawnedNbr;

    // Start is called before the first frame update
    private void Start()
    {
        spawnedNbr = GameObject.Find("SpawnedAgentsNbr").GetComponent<Text>();
        GameObject.Find("MaxAgentsNbr").GetComponent<Text>().text = "Total number of agents in the simulation : " + maxAgentNbr;
        spawn1 = GameObject.Find("Spawner1");
        spawn2 = GameObject.Find("Spawner2");
        InvokeRepeating("Spawn", 0.1f, spawnTime);
    }

    private void Spawn()
    {
        if (nbrSpawnedAgents >= maxAgentNbr)
        {
            this.enabled = false;
            return;
        }

        bool spawner = Random.Range(0, 2) == 1 ? true : false;
        bool character = Random.Range(0, 2) == 1 ? true : false;

        // we always set priority at spawn for avoidance (it goes from 0 to 99)
        if (spawner)
        {
            if (character)
                AllAgents.agents.Add(Instantiate(male, spawn1.transform.position, spawn1.transform.rotation));
            else
                AllAgents.agents.Add(Instantiate(female, spawn1.transform.position, spawn1.transform.rotation));
        }
        else
        {
            if (character)
                AllAgents.agents.Add(Instantiate(male, spawn2.transform.position, spawn2.transform.rotation));
            else
                AllAgents.agents.Add(Instantiate(female, spawn2.transform.position, spawn2.transform.rotation));
        }
        
        nbrSpawnedAgents++;

        spawnedNbr.text = "Number of agents currently spawned : " + nbrSpawnedAgents;
    }
}