using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {

    public static float timeRemain = Data.initTime;

    static GameObject time;
    static Text timeText;

    public bool isPause = false;

    void Start()
    {
        time = GameObject.FindGameObjectWithTag("Time");
        timeText = time.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (isPause)
        {
            timeText.text = "TIME: " + timeRemain.ToString("0");
            if (timeRemain <= 10f)
            {
                timeText.color = Color.red;
            }
            else
            {
                timeText.color = Color.white;
            }
        }
        else
        {
            TimerRun();
        }   
    }

    private static void TimerRun()
    {
        timeRemain -= Time.deltaTime;
        timeText.text = "TIME: " + timeRemain.ToString("0");
        if (timeRemain <= 10f)
        {
            timeText.color = Color.red;
        }
        else
        {
            timeText.color = Color.white;
        }
        if (timeRemain <= 0)
        {
            if (Data.mode == 1)
            {
                timeRemain = 0;
                Data.cod = "RAN OUT OF TIME!";
                SceneManager.LoadScene("Over");
            }
            else
            {
                if (Game2P.score1 == Game2P.score2)
                    Game2P.winner = 0;
                else if (Game2P.score1 > Game2P.score2)
                    Game2P.winner = 1;
                else
                    Game2P.winner = 2;
                Data.cod = "";
                SceneManager.LoadScene("Over2P");
            }
        }
    }
}
