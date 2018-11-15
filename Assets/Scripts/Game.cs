﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Game : MonoBehaviour {

    public static int width = 10;
    public static int height = 20;
    static Text scoreText;
    static Text timeText;
    static float score = 0.0f;
    public static float fallSpeed = 0.4f;
    static int thresh = 50;
    GameObject scoreObj;
    GameObject timeObj;
    GameObject spawnerObj;
    Spawner spawner;

    void Start()
    {
        scoreObj = GameObject.FindGameObjectWithTag("Score");
        if (scoreObj)
        {
            Data.score = 0.0f;
            Data.fallSpeed = 0.4f;
            scoreText = scoreObj.GetComponent<Text>();
            scoreText.text = "SCORE: 0.00";
        }
        
        spawnerObj = GameObject.Find("Spawner");
        if (spawnerObj)
        {
            spawner = spawnerObj.GetComponent<Spawner>();
        }
    }

    // grid: tracks all position of the grid
    public static Transform[,] grid = new Transform[width, height];

    // RoundPosition:
    /* 
     During the rotation of a block, the position might not stay at an integer
     coordinate, so we need this fucntion to keep all the coordinates in 
     integer.
    */
    public static Vector2 RoundPosition(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }

    // InsideBorder:
    // check if the block is inside the borders
    public static bool InsideBorder(Vector2 position)
    {
        return ((int)position.x >= 0 &&
                (int)position.x < width &&
                (int)position.y >= 0);
    }

    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            if(grid[x,y] == null)
            {
                return false;
            }
        }
        return true;
    }

    public static void DeleteRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
            
        }
    }

    public static void DeleteFullRows()
    {
        for (int y = 0; y < height; ++y)
        {
            if(IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                // reset the y after delete the ro in order to keep the index right
                --y;
            }
        }
    }

    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < width; ++x)
            // why ++x?
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y]; // why not grid[x, y] = grid[x, y - 1]
                grid[x, y] = null;
                // Update Block position
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    // 
    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < height; ++i)
        {
            DecreaseRow(i);
        }
    }

    public static void PerformOperations()
    {
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                //Debug.Log("X: " + x + "\tY: " + y + "\t" + grid[x, y]);
                if (grid[x,y] != null)
                {
                    Tile tile = grid[x, y].gameObject.GetComponent<Tile>();
                    int val1 = -1;
                    int val2 = -1;
                    if(tile.type == "operator")
                    {
                        //LR
                        if (x < width - 1 && x != 0)
                        {
                            if (grid[x - 1, y] != null && grid[x + 1, y] != null)
                            {
                                Tile left = grid[x - 1, y].gameObject.GetComponent<Tile>();
                                Tile right = grid[x + 1, y].gameObject.GetComponent<Tile>();
                                if (left.type == "number" && right.type == "number")
                                {
                                    val1 = int.Parse(left.value);
                                    val2 = int.Parse(right.value);
                                }
                            }
                        }

                        if(val1 != -1 && val2 != -1)
                        {
                            float result = CalculateResult(Math.Max(val1, val2), Math.Min(val1, val2), tile.value);
                            if (result != float.PositiveInfinity)
                            {
                                Destroy(grid[x, y].gameObject);
                                grid[x, y] = null;
                                Destroy(grid[x - 1, y].gameObject);
                                grid[x - 1, y] = null;
                                Destroy(grid[x + 1, y].gameObject);
                                grid[x + 1, y] = null;

                                AdjustRows(x, y + 1);
                                UpdateScore(result);
                            }
                        }
                        else
                        {
                            //UD
                            if (y != 0 && y != height - 1)
                            {
                                if (grid[x, y-1] != null && grid[x, y+1] != null)
                                {
                                    Tile up = grid[x, y+1].gameObject.GetComponent<Tile>();
                                    Tile down = grid[x, y-1].gameObject.GetComponent<Tile>();
                                    if (up.type == "number" && down.type == "number")
                                    {
                                        val1 = int.Parse(up.value);
                                        val2 = int.Parse(down.value);
                                    }
                                }
                            }
                            if (val1 != -1 && val2 != -1)
                            {
                                float result = CalculateResult(Math.Max(val1, val2), Math.Min(val1, val2), tile.value);
                                if (result != float.PositiveInfinity)
                                {
                                    Destroy(grid[x, y].gameObject);
                                    grid[x, y] = null;
                                    Destroy(grid[x, y - 1].gameObject);
                                    grid[x, y - 1] = null;
                                    Destroy(grid[x, y + 1].gameObject);
                                    grid[x, y + 1] = null;

                                    AdjustTest(x, y + 2);
                                    //AdjustColumn(x, y);
                                    UpdateScore(result);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    static void UpdateScore(float res)
    {
        Data.score += res;
        scoreText.text = "SCORE: " + Data.score.ToString("0.00");
        if(Data.score >= thresh)
        {
            Data.fallSpeed /= 1.25f;
            Debug.Log(Data.fallSpeed);
            thresh += 50;
            Debug.Log("FASTER!");
        }
    }

    static float CalculateResult(int a, int b, string op)
    {
        if(op == "add")
        {
            return (a + b);
        }
        else if(op == "sub")
        {
            return (a - b);
        }
        else if(op == "mult")
        {
            return (a * b);
        }
        else
        {
            if(b == 0)
            {
                return float.PositiveInfinity;
            }
            return (a / (float)b);
        }
    }

    static void AdjustRows(int col, int row)
    {
        for(int y = row+1; y < height; y++)
        {
            if (grid[col - 1, y] != null)
            {
                grid[col - 1, y - 1] = grid[col - 1, y];
                grid[col - 1, y].position += new Vector3(0, -1, 0);
                grid[col - 1, y] = null;
            }
            if (grid[col, y] != null)
            {
                grid[col, y - 1] = grid[col, y];
                grid[col, y].position += new Vector3(0, -1, 0);
                grid[col, y] = null;  
            }
            if(grid[col+1,y] != null)
            { 
                grid[col + 1, y-1] = grid[col+1, y];
                grid[col + 1, y].position += new Vector3(0, -1, 0);
                grid[col + 1, y] = null;

            }
            
        }
    }

    static void AdjustColumn(int col, int row)
    {
        for(int y = row+2; y < height; y++)
        {
            if(grid[col,y] != null)
            {
                grid[col, y - 3] = grid[col, y];
                grid[col, y].position += new Vector3(0, -3, 0);
                grid[col, y] = null;

            }
              
        }

        /*
        for(int x = 0; x < width; x++)
        {
            Debug.Log("in 2nd loop");
            for(int y = row+2; y < height; y++)
            {
                if(grid[x,y] != null)
                {
                    int r = y;
                    while (grid[x,r-1] == null)
                    {
                        grid[x, r - 1] = grid[x, r];
                        grid[x, r].position += new Vector3(0, -1, 0);
                        r--;
                    }
                }
            }
            
        }

        
        for(int y = row+2; y < height; y++)
        {
            Debug.Log("In 2nd loop");
            for(int x = 0; x < width; x++)
            {
                if(grid[x,y-1] == null && grid[x,y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y].position += new Vector3(0, -1, 0);
                }
            }
        }
        */
    }

    /* 
    >>> AdjustRow + AdjustColumn needs to be changed:
        Modify how to adjust the tiles position above the calculation:
        Decrease all the adjacent tiles above!

       Propose Solution:
       AdjustTest:
       Use BFS or DFS to search through all the tiles that connected above the
       calculation and decrease all tiles' row in the connected tiles above.

       col, row: x,y coordinates of the root tile, the root tile is the tile right above the calculation.
                col and row changed bases on the calculation happened:
                - Horizontal (root.col = x, root.row = y + 1)
                - Vertical  (root.col = x, root.row = y + 1)
    */
    static void AdjustTest (int col, int row)
    {
        // Track all the tiles that has been visited
        Dictionary<Transform, bool> visited = new Dictionary<Transform, bool>();
        // initiate
        foreach (Transform child in grid)
        {
            visited[child] = false;
        }

        Queue<Transform> queue = new Queue<Transform>();

        queue.Enqueue(grid[col, row]);
        visited[grid[col, row]] = true;

        while (queue.Count != 0)
        {
            // pop the queue
            Transform s = queue.Dequeue();
            Debug.Log(s.position);

            // check the adjacent tiles to four direction
            Transform leftTile = grid[col - 1, row];
            Transform rightTile = grid[col + 1, row];
            Transform upperTile = grid[col, row + 1];
            Transform lowerTile = grid[col, row - 1];

            if (leftTile != null)
            {
                if (visited[leftTile] == false)
                {
                    queue.Enqueue(leftTile);
                    visited[leftTile] = true;
                }
            }
            if (rightTile != null)
            {
                if (visited[rightTile] == false)
                {
                    queue.Enqueue(rightTile);
                    visited[rightTile] = true;
                }
            }
            if (upperTile != null)
            {
                if (visited[upperTile] == false)
                {
                    queue.Enqueue(upperTile);
                    visited[upperTile] = true;
                }
            }
            if (lowerTile != null)
            {
                if (visited[lowerTile] == false)
                {
                    queue.Enqueue(lowerTile);
                    visited[lowerTile] = true;
                }
            }
        }

        // The lowest tile in the adjacent tile group
        List<Transform> noBaseTile = new List<Transform>();

         
        // Find all tiles in visited that has no tile under it
        foreach (Transform child in visited.Keys)
        {
            if (visited[child] == true)
            {
                Vector2 underChild = new Vector2(child.position.x, child.position.y - 1);
                Transform underTile = grid[(int)underChild.x, (int)underChild.y];
                if (underChild == null)
                {
                    noBaseTile.Add(child);
                }
            }
        }

        // After we find the tiles that don't have a tile under them. 
        // Calculate the minimal distance that all tiles that need to fall
        int lowestFallDistance = 999;

        foreach (Transform child in noBaseTile)
        {
            int measureY = (int)child.position.y - 1;
            int fallingDistance = 0;
            while (!grid[(int)child.position.x, measureY])
            {
                measureY -= 1;
                fallingDistance += 1;
            }

            if (lowestFallDistance > fallingDistance)
            {
                lowestFallDistance = fallingDistance;
            }
        }

        // Performing fall
        while (lowestFallDistance > 0) {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; x < width; y++)
                {
                    if (visited.ContainsKey(grid[x,y]))
                    {
                        grid[x, y - 1] = grid[x, y];
                        grid[x, y].position += new Vector3(0, -1, 0);
                        grid[x, y] = null;
                    }
                }
            }
        }
    }

    public void Restart()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(grid[x,y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }
        Data.score = 0.0f;
        Data.fallSpeed = 0.4f;
        Data.timeBySec = 5.0f;
        scoreText.text = "SCORE: " + score.ToString("0.00");
        spawner.SpawnNext();
    }
}
