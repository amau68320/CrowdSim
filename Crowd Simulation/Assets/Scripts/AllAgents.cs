using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllAgents : MonoBehaviour
{
    public static List<GameObject> agents;

    void Start()
    {
        agents = new List<GameObject>();
    }
}
