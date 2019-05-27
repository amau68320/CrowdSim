using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPassing : MonoBehaviour
{
    private void OnTriggerExit(Collider perso)
    {
        if (string.Equals(perso.gameObject.name, "male(Clone)") || string.Equals(perso.gameObject.name, "female(Clone)"))
            CheckAllEntered.nbrAgentEntered++;
    }
}
