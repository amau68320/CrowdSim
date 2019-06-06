using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple list to conserve and acces all agents inside the simulation 
public class AllAgents : MonoBehaviour
{
    public static List<GameObject> agents;

    void Start()
    {
        agents = new List<GameObject>();
    }
}
