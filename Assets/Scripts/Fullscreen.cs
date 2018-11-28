﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is originally from Kyle Banks with some modifications
// Project authors have read through the documentation about this script：
// https://kylewbanks.com/blog/create-fullscreen-background-image-in-unity2d-with-spriterenderer

public class Fullscreen : MonoBehaviour {

    // get the size of the background image and camera
    void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector2 scale = transform.localScale;
        if (cameraSize.x >= cameraSize.y)
        { // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        }
        else
        { // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }

        // Optional
        //transform.position = Vector2.zero; 
        transform.localScale = scale;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
