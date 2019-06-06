using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private const int minNbr = 10;
    private const int maxNbr = 80;
    private int old;
    private int nbrAgents;

    void Start()
    {
        old = 80;
        nbrAgents = 80;
    }
    public void Play()
    {
        // we reset the static variables (it's useful when we restart the simulation)
        AgentSpawn.maxAgentNbr = nbrAgents;
        DoorPassing.isEvacuation = false;
        AgentManager.ResetOnTalk();
        AgentEvacuation.ResetOnAskHelp();
        TriggerAlarm.ResetOnAlarm();
        TriggerAlarm.ResetOnClose();
        TriggerAlarm.nbrAgentsInRoom = AgentSpawn.maxAgentNbr;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ModifyAgentNbr()
    {
        // Check if we enter a correct number of agents 
        if (!int.TryParse(GameObject.Find("NbrAgents").GetComponent<InputField>().text, out nbrAgents))
        {
            nbrAgents = old;
            GameObject.Find("NbrAgents").GetComponent<InputField>().text = old.ToString();
        }
        else if ((nbrAgents < minNbr) || (nbrAgents > maxNbr))
        {
            nbrAgents = old;
            GameObject.Find("NbrAgents").GetComponent<InputField>().text = old.ToString();
        }
        else
            old = nbrAgents;
    }
}
