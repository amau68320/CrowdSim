using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
