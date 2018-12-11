using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour {

    public float zoomToSize = 5f;
    public Vector3 originalCamaraPos;
    public float originalCamaraSize = 8f;

	// Use this for initialization
	void Start () 
    {
        originalCamaraPos = new Vector3(4.5f, 7.5f, -20f);
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

    IEnumerator MoveAndZoomTo(Vector3 operationPosition)
    {
        transform.position = Vector3.Lerp(transform.position, operationPosition, 0.6f);
        GetComponent<Camera>().orthographicSize = zoomToSize;
        yield return new WaitForSeconds(2);
        transform.position = originalCamaraPos;
        GetComponent<Camera>().orthographicSize = originalCamaraSize;
    }
}
