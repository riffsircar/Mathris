using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Game : MonoBehaviour {

    public static int width = 10;
    public static int height = 20;
    static Text scoreText;
    static float score = 0.0f;

    void Start()
    {
        Debug.Log("Start");
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        scoreText.text = "SCORE: 0";
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
            for(int x = 1; x < width-1; x++)
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
                        if(grid[x-1,y] != null && grid[x+1,y] != null)
                        {
                            Tile left = grid[x-1, y].gameObject.GetComponent<Tile>();
                            Tile right = grid[x+1, y].gameObject.GetComponent<Tile>();
                            if(left.type == "number" && right.type == "number")
                            {
                                val1 = int.Parse(left.value);
                                val2 = int.Parse(right.value);
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
                                AdjustRows(x, y);
                                score += result;
                                scoreText.text = "SCORE: " + score.ToString();
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
                                    AdjustColumn(x, y);
                                    score += result;
                                    scoreText.text = "SCORE: " + score.ToString();
                                }
                            }
                        }
                    }
                }
            }
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
    }
}
