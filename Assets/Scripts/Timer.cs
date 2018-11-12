using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {

    public float timeRemain = Data.timeBySec;

    static GameObject time;
    static Text timeText;

    void Start()
    {
        time = GameObject.FindGameObjectWithTag("Time");
        timeText = time.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update ()
    {
        timeRemain -= Time.deltaTime;
        timeText.text = "TIME: " + timeRemain.ToString("0.00");
        if (timeRemain < 0)
        {
            timeRemain = 0;
            Debug.Log("Time!");
            SceneManager.LoadScene("Over");
        }
	}
}
