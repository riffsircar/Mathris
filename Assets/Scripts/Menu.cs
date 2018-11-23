using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	// Use this for initialization
    void Start()
    {
        GameObject finalScore = GameObject.Find("FinalScore");
        if(finalScore)
        {
            finalScore.GetComponent<Text>().text = "Score: " + Data.score.ToString("0.00");
        }
    }

	public void PlayGame()
    {
        Timer.timeRemain = 120f;
        SceneManager.LoadScene("Main");
    }   

    public void QuitGame()
    {
        Application.Quit();
    }
}
