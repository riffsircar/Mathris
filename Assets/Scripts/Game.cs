using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static int width = 10;
    public static int height = 20;

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

    // InsideBoarder:
    // check if the block is inside the boarders
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
        Debug.Log("PERFORM OPS");
        for(int y = 0; y < height; y++)
        {
            for(int x = 1; x < width-1; x++)
            {
                Debug.Log("X: " + x + "\tY: " + y + "\t" + grid[x, y]);
                if (grid[x,y] != null)
                {
                    Debug.Log("Type: " + grid[x,y].gameObject.GetComponent<Tile>().type + "\tValue: " + grid[x, y].gameObject.GetComponent<Tile>().value);
                }
            }
        }
    }


}
