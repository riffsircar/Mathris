using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	// Use this for initialization
	public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }   

    public void QuitGame()
    {
        Application.Quit();
    }
}
