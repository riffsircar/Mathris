using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCalSounds : MonoBehaviour {
    // Audio
    AudioSource audioSource;
    AudioClip audioClip;
    Game game;

    // Use this for initialization
    void Start () {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound(string op)
    {
        if (op == "+")
        {
            audioClip = GameObject.Find("Adding Sound").GetComponent<AudioClip>();

        }
        else if (op == "-")
        {
            audioClip = GameObject.Find("Subtraction Sound").GetComponent<AudioClip>();

        }
        else if (op == "*")
        {
            audioClip = GameObject.Find("Multiplication Sound").GetComponent<AudioClip>();

        }
        else
        {
            audioClip = GameObject.Find("Division Sound").GetComponent<AudioClip>();

        }
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
