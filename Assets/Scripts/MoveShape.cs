using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveShape : MonoBehaviour {

    float lastFall = 0;
    Spawner spawner;
    Spawner2P spawner2P;
    float fallSpeed = 0.3f;
    public static bool isLanded = false;
    public static bool isLandedP1 = false;
    public static bool isLandedP2 = false;

    /*
    void Awake()
    {
        
    }
    */
    // Use this for initialization
    void Start ()
    {
        Debug.Log("moveshape start");
        //   Debug.Log("Inside MoveShape start");
        // If the default is not valid, that means the blocks have reached the top
        // Which means game over
        if (!isValidGridPos())
        {
            Debug.Log("Game Over");
            Data.cod = "OVERFLOW!";
            if(Data.mode == 1)
                SceneManager.LoadScene("Over");
            else
            {
                Game2P.winner = 2;
                SceneManager.LoadScene("Over2P");
            }
        }

        if (Data.mode == 1)
        {  
            spawner = FindObjectOfType<Spawner>();
        }
        else
        {
            spawner2P = GameObject.Find("SpawnerP1").GetComponent<Spawner2P>();
        }
        //Debug.Log("Exiting MoveShape start");
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

            if (isLanded)
            {
            Debug.Log("P1 landed");
                PerformCalAndSpawn(transform);
                isLanded = false;
            }
	}

    // LeftMove:
    // Use keyboard to control the block move to left
    private void LeftMove()
    {
            transform.position += new Vector3(-1, 0, 0);

            if (isValidGridPos())
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
        if (Data.mode == 1)
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
        else
        {
            Game2P.PerformOperations(t, Game2P.grid1,1);
           Game2P.DeleteFullRows(1);
            spawner2P.SpawnNext();
        }
    }

    public bool isValidGridPos()
    {
        if (Data.mode == 1)
        {
            //Debug.Log("Inside isValidGridPos");
            foreach (Transform child in transform)
            {
                Vector2 pos = Game.RoundPosition(child.position);

                // detect if the block is inside border or not
                if (!Game.InsideBorder(pos))
                {
                    Debug.Log("1st valid if");
                    Debug.Log(pos);
                    return false;
                }

                // Used in rotation: find the block that is in the position already.
                // If there is a block that at the position, return false.
                if (Game.grid[(int)pos.x, (int)pos.y] != null &&
                    Game.grid[(int)pos.x, (int)pos.y].parent != transform)
                {
                    Debug.Log("2nd valid if");
                    Debug.Log(pos);
                    return false;
                }
            }
            //Debug.Log("Exiting isValidGridPos");
            return true;
        }
        else
        {
            foreach (Transform child in transform)
            {
                Vector2 pos = Game2P.RoundPosition(child.position);

                // detect if the block is inside border or not
                if (!Game2P.InsideGridOneBorder(pos))
                {
                    Debug.Log("1st valid if");
                    Debug.Log(pos);
                    return false;
                }

                // Used in rotation: find the block that is in the position already.
                // If there is a block that at the position, return false.
                if (Game2P.grid1[(int)pos.x, (int)pos.y] != null &&
                    Game2P.grid1[(int)pos.x, (int)pos.y].parent != transform)
                {
                    Debug.Log("2nd valid if");
                    Debug.Log(pos);
                    return false;
                }

                /*
                // Used in rotation: find the block that is in the position already.
                // If there is a block that at the position, return false.
                if (Game.grid[(int)pos.x, (int)pos.y] != null &&
                    Game.grid[(int)pos.x, (int)pos.y].parent != transform)
                {
                    Debug.Log("2nd valid if");
                    return false;
                }
                */

                // Add if the 
            }
            //Debug.Log("Exiting isValidGridPos");
            return true;
        }
        return true;
    }

    public bool isValidGridOnePos()
    {
        
            Debug.Log("Inside isValidGridOnePos");
            foreach (Transform child in transform)
            {
                Vector2 pos = Game2P.RoundPosition(child.position);

                // detect if the block is inside border or not
                if (!Game2P.InsideGridOneBorder(pos))
                {
                    Debug.Log("1st valid if");
                    return false;
                }

            // Used in rotation: find the block that is in the position already.
            // If there is a block that at the position, return false.
                if (Game2P.grid1[(int)pos.x, (int)pos.y] != null &&
                    Game2P.grid1[(int)pos.x, (int)pos.y].parent != transform)
                {
                    Debug.Log("2nd valid if");
                    return false;
                }

                /*
                // Used in rotation: find the block that is in the position already.
                // If there is a block that at the position, return false.
                if (Game.grid[(int)pos.x, (int)pos.y] != null &&
                    Game.grid[(int)pos.x, (int)pos.y].parent != transform)
                {
                    Debug.Log("2nd valid if");
                    return false;
                }
                */

                // Add if the 
            }
            //Debug.Log("Exiting isValidGridPos");
            return true;
    }

    // update all grids that belongs to a certain shape.
    void UpdateGrid()
    {
        if (Data.mode == 1)
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
        else
        {
            for (int x = 0; x < Game2P.width; ++x)
            {
                for (int y = 0; y < Game2P.height; ++y)
                {
                    if (Game2P.grid1[x, y] != null) // if has a grid
                    {
                        if (Game2P.grid1[x, y].parent == transform) // if the parent is the shape
                        {
                            Game2P.grid1[x, y] = null;
                        }
                    }
                }
            }

            // put new block in the position
            foreach (Transform child in transform)
            {
                Vector2 pos = Game2P.RoundPosition(child.position);
                Game2P.grid1[(int)pos.x, (int)pos.y] = child;

            }
        }
    }
}
