using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    [SerializeField] float rollingSpeed = 0.05f;
    Material background;
    Vector2 offset;

	// Use this for initialization
	void Start () {
        background = GetComponent<Renderer>().material;
        offset = new Vector2(rollingSpeed, 0);
	}
	
	// Update is called once per frame
	void Update () {
        background.mainTextureOffset += offset * Time.deltaTime;
	}
}
