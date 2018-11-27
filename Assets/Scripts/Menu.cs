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
        GameObject cod = GameObject.Find("COD");
        if(finalScore)
        {
            finalScore.GetComponent<Text>().text = "Score: " + Data.score.ToString("0.00");
        }
        if(cod)
        {
            cod.GetComponent<Text>().text = "Cause of Death: " + Data.cod;
        }
        Game.Reset();
        Debug.Log(Data.score + "\t" + Data.cod);
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
