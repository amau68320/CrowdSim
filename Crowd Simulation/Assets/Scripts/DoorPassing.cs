using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPassing : MonoBehaviour
{
    public static bool isEvacuation = false;
    private void OnTriggerExit(Collider perso)
    {
        if ((string.Equals(perso.gameObject.name, "male(Clone)") || string.Equals(perso.gameObject.name, "female(Clone)")) && !isEvacuation)
            CheckAllEntered.nbrAgentEntered++;

        if ((string.Equals(perso.gameObject.name, "male(Clone)") || string.Equals(perso.gameObject.name, "female(Clone)")) && isEvacuation)
            Destroy(perso.gameObject);
    }
}
