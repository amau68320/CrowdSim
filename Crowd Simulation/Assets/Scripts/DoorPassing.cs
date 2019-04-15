using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPassing : MonoBehaviour
{
    private void OnTriggerExit()
    {
        CheckAllEntered.nbrAgentEntered++;
    }
}
