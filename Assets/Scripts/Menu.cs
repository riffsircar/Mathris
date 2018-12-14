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
        GameObject winner = GameObject.Find("Winner");
        GameObject p1 = GameObject.Find("P1");
        GameObject p2 = GameObject.Find("P2");
        GameObject mainSound = GameObject.Find("Tetris");
        if (finalScore)
        {
            finalScore.GetComponent<Text>().text = "Score: " + Data.score.ToString("0.00");
        }
        if(cod)
        {
            if (Data.mode == 1)
                cod.GetComponent<Text>().text = "Cause of Death: " + Data.cod;
            else
                cod.GetComponent<Text>().text = Data.cod;
        }
        if(winner)
        {
            string w = Game2P.winner == 1 ? "Player 1" : "Player 2";
            winner.GetComponent<Text>().text = w + " wins!";
        }
        if(p1 && p2)
        {
            p1.GetComponent<Text>().text = "Player 1: " + Game2P.score1;
            p2.GetComponent<Text>().text = "Player 2: " + Game2P.score2;
        }
        if (mainSound)
        {
            string scene = SceneManager.GetActiveScene().name;
            if(scene == "Over" || scene == "Over2P")
                mainSound.GetComponent<AudioSource>().Stop();
        }
        Game2P.Reset();
    }

    public void PlayRule2P()
    {
        Data.mode = 2;
        SceneManager.LoadScene("Instructions2P");
    }

    public void PlayGame()
    {
        Timer.timeRemain = 120f;
        SceneManager.LoadScene("Main");
    }

    public void Play2P()
    {
        Timer.timeRemain = 120f;
        SceneManager.LoadScene("TwoPlayer");
    }

    public void StartGame()
    {
        Data.mode = 1;
        SceneManager.LoadScene("Instructions");
    }

    public void Start2P()
    {
        Data.mode = 2;
        SceneManager.LoadScene("Instructions2P(0)");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
