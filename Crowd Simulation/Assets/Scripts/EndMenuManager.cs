using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This script manages the Menu at the end of the simulation 
public class EndMenuManager : MonoBehaviour
{
    void Start()
    {
        GameObject.Find("EndText").GetComponent<Text>().text = TriggerAlarm.endText;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene("Menu");
    }
}
