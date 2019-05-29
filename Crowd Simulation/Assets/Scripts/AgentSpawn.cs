using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawn : MonoBehaviour
{
    public GameObject male;
    public GameObject female;
    public float spawnTime = 0.3f;
    public static int maxAgentNbr = 80;
    int nbrSpawnedAgents = 0;
    GameObject spawn1;
    GameObject spawn2;

    // Start is called before the first frame update
    void Start()
    {
        spawn1 = GameObject.Find("Spawner1");
        spawn2 = GameObject.Find("Spawner2");
        InvokeRepeating("Spawn", 0.1f, spawnTime);
    }

    void Spawn()
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
    }
}