using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVisualEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Normal Destroy
    public void Normal ()
    {
        StartCoroutine(DelayForSecond(time));
    }

    IEnumerator DelayForSecond(float time)
    {
        Debug.Log("FUCK Static!!!");
        yield return new WaitForSeconds(time);
    }
}
