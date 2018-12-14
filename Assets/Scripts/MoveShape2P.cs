using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveShape2P : MonoBehaviour {

    float lastFall = 0;
    Spawner2P spawner;
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
         //   Debug.Log("Game Over");
            Data.cod = "OVERFLOW!";
            Game2P.winner = 1;
            SceneManager.LoadScene("Over2P");
        }
        spawner = GameObject.Find("SpawnerP2").GetComponent<Spawner2P>();

        foreach(Transform child in transform)
        {
            Vector2 pos = Game.RoundPosition(child.position);
            if (pos.y < 14)
            {
                Data.cod = "OVERFLOW!";
                Game2P.winner = 1;
                SceneManager.LoadScene("Over2P");
            }   
        }
    }

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LeftMove();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightMove();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotation();
        }

        if (Time.time - lastFall >= Data.fallSpeed)
        {
            Fall();
        }

        // The Fall function will be updated by time
        if (Input.GetKeyDown(KeyCode.DownArrow))
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
        Game2P.PerformOperations(t,Game2P.grid2,2);
        // Clean the full rows
        Game2P.DeleteFullRows(2);

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
            Vector2 pos = Game2P.RoundPosition(child.position);
                // detect if the block is inside border or not
                if (!Game2P.InsideGridTwoBorder(pos))
                {
                    return false;
                }

            // Used in rotation: find the block that is in the position already.
            // If there is a block that at the position, return false.
                if (Game2P.grid2[(int)(pos.x - 14), (int)pos.y] != null &&
                    Game2P.grid2[(int)(pos.x - 14), (int)pos.y].parent != transform)
                {
                    return false;
                }
            
        }
        //Debug.Log("Exiting isValidGridPos");
        return true;
    }
    
    // update all grids that belongs to a certain shape.
    void UpdateGrid() 
    {
        // Remove old children from grid
        for (int x = 0; x < Game2P.width; ++x)
        {
            for (int y = 0; y < Game2P.height; ++y)
            {
                if (Game2P.grid2[x, y] != null) // if has a grid
                {
                    if (Game2P.grid2[x, y].parent == transform) // if the parent is the shape
                    {
                        Game2P.grid2[x, y] = null;
                    }
                }
            }
        }

        // put new block in the position
        foreach (Transform child in transform)
        {
            Vector2 pos = Game2P.RoundPosition(child.position);
            Game2P.grid2[(int)(pos.x-14), (int)pos.y] = child;
        }
    }
    
}
