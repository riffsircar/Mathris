using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour {

    public float zoomToSize = 5f;
    public Vector3 originalCamaraPos;
    public float originalCamaraSize = 8f;
    public int secondToPause = 2;
    // Call the Spawner and timer in the game to pause them.
    GameObject spawner;
    GameObject timer;
    // The Game game object is only for the tutorial purpose.
    Game game;

	// Use this for initialization
	void Start () 
    {
        originalCamaraPos = new Vector3(4.5f, 7.5f, -20f);
        spawner = GameObject.Find("Spawner");
        timer = GameObject.Find("Time");
	}

    // Update is called once per frame
    public void ChangeCamera (Vector3 pos) 
    {
        // Adjust the Camara position
        if (pos.y < 3f)
        {
            pos.y += 1f;
        }
        else if (pos.y < 2f)
        {
            pos.y += 2.5f;
        }
        else if (pos.y < 1f)
        {
            pos.y += 5f;
        }
        StartCoroutine(MoveAndZoomTo(pos));
    }

    // This function is for tutorial purpose only
    // all 'isPause'
    IEnumerator MoveAndZoomTo(Vector3 operationPosition)
    {
        transform.position = Vector3.Lerp(transform.position, operationPosition, 0.6f);
        GetComponent<Camera>().orthographicSize = zoomToSize;
        spawner.GetComponent<Spawner>().isPause = true;
        timer.GetComponent<Timer>().isPause = true;

        // Call the tutorial function here

        yield return new WaitForSeconds(secondToPause);

        // Enlarge the camera back to the original position and size.
        transform.position = originalCamaraPos;
        GetComponent<Camera>().orthographicSize = originalCamaraSize;

        // Enable the Spawner and Timer
        spawner.GetComponent<Spawner>().isPause =false;
        //Debug.Log("Current spawner: " + spawner.GetComponent<Spawner>().isPause);
        timer.GetComponent<Timer>().isPause = false;

    }
}
