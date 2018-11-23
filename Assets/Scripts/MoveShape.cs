using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveShape : MonoBehaviour {

    float lastFall = 0;
    Spawner spawner;
    float fallSpeed = 0.3f;
    public static bool isLanded = false;

    /*
    void Awake()
    {
        
    }
    */
	// Use this for initialization
	void Start ()
    {
        // If the default is not valid, that means the blocks have reached the top
        // Which means game over
        if (!isValidGridPos())
        {
            Debug.Log("Game Over");
            SceneManager.LoadScene("Over");
        }
        spawner = FindObjectOfType<Spawner>();
    }

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            LeftMove();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            RightMove();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Rotation();
        }

        if (Time.time - lastFall >= Data.fallSpeed)
        {
            Fall();
        }

        // The Fall function will be updated by time
        if (Input.GetKeyDown(KeyCode.S))
        {
            FastFall();
        }

        if (isLanded == true)
        {
            PerformCalAndSpawn(transform);
            isLanded = false;
        }
	}

    // LeftMove:
    // Use keyboard to control the block move to left
    private void LeftMove()
    {
        transform.position += new Vector3(-1, 0, 0);

        if(isValidGridPos())
        {
            UpdateGrid();
        }

        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    private void RightMove()
    {

        transform.position += new Vector3(1, 0, 0);

        if (isValidGridPos())
        {
            UpdateGrid();
        }

        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    private void Rotation()
    {
        transform.Rotate(0, 0, -90);

        foreach(Transform child in transform)
        {
            child.transform.Rotate(0, 0, 90);
        }

        if (isValidGridPos())
        {
            UpdateGrid();
        }
        else
        {
            transform.Rotate(0, 0, 90);
            foreach (Transform child in transform)
            {
                child.transform.Rotate(0, 0, -90);
            }
        }
    }

    private void Fall()
    {
        // Move downwards
        transform.position += new Vector3(0, -1, 0);

        if (isValidGridPos())
        {
            UpdateGrid();
        }

        else
        {
            transform.position += new Vector3(0, 1, 0);
            enabled = false;
            isLanded = true;
        }
        lastFall = Time.time;
    }

    void FastFall()
    {
        transform.position += new Vector3(0, -1, 0);
        if (isValidGridPos())
        {
            UpdateGrid();
            transform.position += new Vector3(0, -1, 0);
            if (isValidGridPos())
            {
                UpdateGrid();
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);
                enabled = false;
                isLanded = true;
            }
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);
            enabled = false;
            isLanded = true;
        }
        lastFall = Time.time;
    }

    // perform calculation and spawn
    private void PerformCalAndSpawn(Transform t)
    {
        Game.PerformOperations(t);
        // Clean the full rows
        Game.DeleteFullRows();

        // Spawn next shape
        //FindObjectOfType<Spawner>().SpawnNext();
        spawner.SpawnNext();

        // Disable the script of the obj, since it needs to be stop from controlling
        // enabled = false;
    }

    public bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 pos = Game.RoundPosition(child.position);

            // detect if the block is inside border or not
            if (!Game.InsideBorder(pos))
            {
                return false;
            }

            // Used in rotation: find the block that is in the position already.
            // If there is a block that at the position, return false.
            if (Game.grid[(int)pos.x, (int)pos.y] != null &&
                Game.grid[(int)pos.x, (int)pos.y].parent != transform)
            {
                return false;
            }

            // Add if the 
        }
        return true;
    }
    
    // update all grids that belongs to a certain shape.
    void UpdateGrid() 
    {
        // Remove old children from grid
        for (int x = 0; x < Game.width; ++x)
        {
            for (int y = 0; y < Game.height; ++y)
            {
                if (Game.grid[x, y] != null) // if has a grid
                {
                    if (Game.grid[x, y].parent == transform) // if the parent is the shape
                    {
                        Game.grid[x, y] = null;
                    }
                }
            }
        }

        // put new block in the position
        foreach (Transform child in transform)
        {
            Vector2 pos = Game.RoundPosition(child.position);
            Game.grid[(int)pos.x, (int)pos.y] = child;

        }
    }
    
}
