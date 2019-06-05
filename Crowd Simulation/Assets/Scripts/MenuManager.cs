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

    void Start()
    {
        old = 80;
    }
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ModifyAgentNbr()
    {
        if (!int.TryParse(GameObject.Find("NbrAgents").GetComponent<InputField>().text, out AgentSpawn.maxAgentNbr))
        {
            AgentSpawn.maxAgentNbr = old;
            GameObject.Find("NbrAgents").GetComponent<InputField>().text = old.ToString();
        }
        else if ((AgentSpawn.maxAgentNbr < minNbr) || (AgentSpawn.maxAgentNbr > maxNbr))
        {
            AgentSpawn.maxAgentNbr = old;
            GameObject.Find("NbrAgents").GetComponent<InputField>().text = old.ToString();
        }
        else
            old = AgentSpawn.maxAgentNbr;
    }
}
