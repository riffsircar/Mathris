using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Game2P : MonoBehaviour {

    public static int width = 10;
    public static int height = 20;
    static Text scoreText1;
    static Text scoreText2;
    static Text opText1;
    static Text opText2;
    static float score1 = 0.0f;
    static float score2 = 0.0f;
    public static float fallSpeed = 0.4f;
    static int thresh = 50;
    GameObject scoreObj1;
    GameObject scoreObj2;
    GameObject opObj1;
    GameObject opObj2;
    GameObject spawnerObj1;
    GameObject spawnerObj2;
    Spawner spawner1;
    Spawner spawner2;
    static string operationText1;
    static string operationText2;
    public static int plusCountP1 = 0;
    public static int subCountP1 = 0;
    public static int mulCountP1 = 0;
    public static int divCountP1 = 0;
    public static int plusCountP2 = 0;
    public static int subCountP2 = 0;
    public static int mulCountP2 = 0;
    public static int divCountP2 = 0;
    public static int scoreIncrement = 10;

    // grid: tracks all position of the grid
    public static Transform[,] grid = new Transform[width, height];
    public static Transform[,] grid1 = new Transform[width, height];
    public static Transform[,] grid2 = new Transform[width, height];

    void Start()
    {
        scoreObj1 = GameObject.FindGameObjectWithTag("ScoreP1");
        scoreObj2 = GameObject.FindGameObjectWithTag("ScoreP2");
        opObj1 = GameObject.FindGameObjectWithTag("OperationP1");
        opObj2 = GameObject.FindGameObjectWithTag("OperationP2");
        if (scoreObj1 && scoreObj2)
        {
            Data.score = 0.0f;
            Data.fallSpeed = 0.4f;
            Data.goal = 10;
            scoreText1 = scoreObj1.GetComponent<Text>();
            scoreText1.text = "SCORE:\n0.00";
            scoreText2 = scoreObj2.GetComponent<Text>();
            scoreText2.text = "SCORE:\n0.00";
            opText1 = opObj1.GetComponent<Text>();
            opText1 = opObj2.GetComponent<Text>();
        }
        
        spawnerObj1 = GameObject.Find("SpawnerP1");
        spawnerObj2 = GameObject.Find("SpawnerP2");
        if (spawnerObj1)
        {
            spawner1 = spawnerObj1.GetComponent<Spawner>();
        }
        if (spawnerObj2)
        {
            spawner2 = spawnerObj2.GetComponent<Spawner>();
        }
        Reset();
    }


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
        return true;
            return ((int)position.x >= 0 &&
                    (int)position.x < width &&
                    (int)position.y >= 0);
    }

    public static bool InsideGridOneBorder(Vector2 position)
    {
        Debug.Log("Grid1 bord");
        //Debug.Log(position.x + " " + position.y);
        return ((int)position.x >= 0 &&
                (int)position.x < width &&
                (int)position.y >= 0);
    }

    public static bool InsideGridTwoBorder(Vector2 position)
    {
        Debug.Log("Grid2 bord");
        //Debug.Log(position.x + " " +  position.y);
        return ((int)position.x >= 14 &&
                (int)position.x < (14+width) &&
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
        int lines = 0;
        for (int y = 0; y < height; ++y)
        {
            if(IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                // reset the y after delete the ro in order to keep the index right
                --y;
                lines++;
            }
        }
        UpdateScore(lines * 5.0f);
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
 
    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < height; ++i)
        {
            DecreaseRow(i);
        }
    }

    public static void PerformOperations(Transform t)
    {
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                //Debug.Log("X: " + x + "\tY: " + y + "\t" + grid[x, y]);
                if (grid[x,y] != null)
                {

                    Tile tile = grid[x, y].gameObject.GetComponent<Tile>();
                    int val_l = -1;
                    int val_r = -1;
                    int val_u = -1;
                    int val_d = -1;

                    if (tile.type == "operator")
                    {
                        // Four directions calculation
                        if (x < width - 1 && x != 0 && y != 0 && y != height - 1)
                        {
                            if (grid[x - 1, y] != null && grid[x + 1, y] != null &&
                                grid[x, y - 1] != null && grid[x, y + 1] != null)
                            {
                                //Debug.Log("BOTH DIRS");
                                Tile left = grid[x - 1, y].gameObject.GetComponent<Tile>();
                                Tile right = grid[x + 1, y].gameObject.GetComponent<Tile>();
                                Tile up = grid[x, y + 1].gameObject.GetComponent<Tile>();
                                Tile down = grid[x, y - 1].gameObject.GetComponent<Tile>();

                                if (left.type == "number" && right.type == "number" &&
                                    up.type == "number" && down.type == "number")
                                {
                                    val_l = int.Parse(left.value);
                                    val_r = int.Parse(right.value);
                                    val_u = int.Parse(up.value);
                                    val_d = int.Parse(down.value);
                                }
                            }
                        }

                        if (val_l != -1 && val_r != -1 && val_u != -1 && val_d != -1)
                        {
                            
                            float resultLR = CalculateResult(val_l, val_r, tile.value);
                            float resultUD = CalculateResult(val_u, val_d, tile.value);

                            float result = (resultLR + resultUD); //doubling the result
                            if (result != float.PositiveInfinity)
                            {
                                operationText1 = "(" + val_l.ToString() + " " + tile.value + " " + val_r.ToString() + ") + (" + val_u.ToString() + " " + tile.value + " " + val_d.ToString() + ")\n = " + result.ToString("0.00") + "\nDOUBLE OP!";
                                opText1.text = "OPERATION: \n" + operationText1;
                                //Debug.Log("OP: " + operationText);
                                Destroy(grid[x, y].gameObject);
                                DestroyWithParticleEffect(grid[x, y].position, tile.particleEffect);
                                grid[x, y] = null;
                                Destroy(grid[x - 1, y].gameObject);
                                grid[x - 1, y] = null;
                                Destroy(grid[x + 1, y].gameObject);
                                grid[x + 1, y] = null;

                                //Destroy(grid[x, y].gameObject);
                                //grid[x, y] = null;
                                Destroy(grid[x, y - 1].gameObject);
                                grid[x, y - 1] = null;
                                Destroy(grid[x, y + 1].gameObject);
                                grid[x, y + 1] = null;

                                AdjustRows(x, y);
                                Fix(x, y, t);
                                UpdateScore(result*2.0f);
                            }
                            else
                            {
                                Data.cod = "DIVIDE BY ZERO!";
                                SceneManager.LoadScene("Over");
                            }
                        }
                        else
                        {
                            //Debug.Log("LEFT RIGHT");
                            //Left and Right calculation
                            if (x < width - 1 && x != 0)
                            {
                                if (grid[x - 1, y] != null && grid[x + 1, y] != null)
                                {
                                    Tile left = grid[x - 1, y].gameObject.GetComponent<Tile>();
                                    Tile right = grid[x + 1, y].gameObject.GetComponent<Tile>();
                                    if (left.type == "number" && right.type == "number")
                                    {
                                        val_l = int.Parse(left.value);
                                        val_r = int.Parse(right.value);
                                    }
                                }
                            }

                            if (val_l != -1 && val_r != -1)
                            {
                                //float result = CalculateResult(Math.Max(val1, val2), Math.Min(val1, val2), tile.value);
                                float result = CalculateResult(val_l, val_r, tile.value);
                                if (result != float.PositiveInfinity)
                                {
                                    operationText1 = "(" + val_l.ToString() + " " + tile.value + " " + val_r.ToString() + ") = " + result.ToString("0.00");
                                    //Debug.Log("OP: " + operationText);
                                    opText1.text = "OPERATION: \n" + operationText1;
                                    Destroy(grid[x, y].gameObject);
                                    DestroyWithParticleEffect(grid[x, y].position, tile.particleEffect);
                                    grid[x, y] = null;
                                    Destroy(grid[x - 1, y].gameObject);
                                    grid[x - 1, y] = null;
                                    Destroy(grid[x + 1, y].gameObject);
                                    grid[x + 1, y] = null;

                                    AdjustRows(x, y);
                                    Fix(x, y, t);
                                    //AdjustTest(x, y + 1);
                                    UpdateScore(result);
                                }
                                else
                                {
                                    Data.cod = "DIVIDE BY ZERO!";
                                    SceneManager.LoadScene("Over");
                                }
                            }
                            else
                            {
                                //Up and Down calculation
                                //Debug.Log("UP DOWN");

                                if (y != 0 && y != height - 1)
                                {
                                    if (grid[x, y - 1] != null && grid[x, y + 1] != null)
                                    {
                                        Tile up = grid[x, y + 1].gameObject.GetComponent<Tile>();
                                        Tile down = grid[x, y - 1].gameObject.GetComponent<Tile>();
                                        if (up.type == "number" && down.type == "number")
                                        {
                                            val_u = int.Parse(up.value);
                                            val_d = int.Parse(down.value);
                                        }
                                    }
                                }
                                if (val_u != -1 && val_d != -1)
                                {
                                    
                                    //float result = CalculateResult(Math.Max(val1, val2), Math.Min(val1, val2), tile.value);
                                    float result = CalculateResult(val_u, val_d, tile.value);
                                    if (result != float.PositiveInfinity)
                                    {
                                        operationText1 = "(" + val_u.ToString() + " " + tile.value + " " + val_d.ToString() + ") = " + result.ToString("0.00");
                                        //Debug.Log("OP: " + operationText);
                                        opText1.text = "OPERATION: \n" + operationText1;
                                        Destroy(grid[x, y].gameObject);
                                        DestroyWithParticleEffect(grid[x, y].position, tile.particleEffect);
                                        grid[x, y] = null;
                                        Destroy(grid[x, y - 1].gameObject);
                                        grid[x, y - 1] = null;
                                        Destroy(grid[x, y + 1].gameObject);
                                        grid[x, y + 1] = null;

                                        //AdjustTest(x, y + 2);
                                        Fix(x, y, t);
                                        UpdateScore(result);
                                    }
                                    else
                                    {
                                        Data.cod = "DIVIDE BY ZERO!";
                                        SceneManager.LoadScene("Over");
                                    }
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
        scoreText1.text = "SCORE:\n" + Data.score.ToString("0.00");
        // When player reach the goal
        if(Data.score >= Data.goal)
        {
            // Change the speed
            Data.fallSpeed /= 1.025f;
            Debug.Log(Data.fallSpeed);
            Debug.Log("FASTER!");

            // Change the goal and time
            //Data.goal = (int)Data.score + scoreIncrement;
            //goalText.text = "NEXT GOAL:\n" + Data.goal.ToString();
            Timer.timeRemain += 90f;
        }
    }

    static float CalculateResult(int a, int b, string op)
    {
        /*
        float result;
        if(op == "+")
        {
            plusCount++;
            result = (a + b);
        }
        else if(op == "-")
        {
            subCount++;
            result = (a - b);
        }
        else if(op == "*")
        {
            mulCount++;
            result = (a * b);
        }
        else
        {
            if (b == 0)
            {
                result = float.PositiveInfinity;
            }
            else
            {
                divCount++;
                result = (a / (float)b);
            }
        }
        Debug.Log(plusCount + "\t" + subCount + "\t" + mulCount + "\t" + divCount);
        Spawner.UpdateUnlockedOperators();
        return result;
        */
        return 0;
    }

    static void DestroyWithParticleEffect(Vector3 tilePosition, GameObject particleEffect)
    {
        //Destroy(tile.gameObject);
        Vector3 adjust = new Vector3(0, 0, -15);
        GameObject particleSystem = Instantiate
            (particleEffect, tilePosition + adjust, Quaternion.identity);
        Destroy(particleSystem, 1f);
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

    static void Fix(int col, int row, Transform t)
    {
        List<int> xs = new List<int>();
        List<int> ys = new List<int>();

        foreach(Transform child in t)
        {
            
            int x = (int)Math.Round(child.position.x);
            int y = (int)Math.Round(child.position.y);
            //Debug.Log("X: " + child.position.x + "\t" + "Y: " + child.position.y);
                xs.Add(x);
                ys.Add(y);
        }
        // Sort y in order to let tiles fall at bottom first.
        //Debug.Log("count: " + ys.Count);
        ys.Sort();
        int r = ys[0];
        int r_max = ys[ys.Count - 1];
        //Debug.Log("R: " + r);
        foreach(int x in xs)
        {
            //Debug.Log("X: " + x);
            for (int j = r; j <= r_max; j++)
            {
                //Debug.Log("J: " + j);
                if (grid[x, j] != null)
                {
                    int temp_j = j;
                    while (temp_j > 0 && grid[x, temp_j - 1] == null)
                    {
                        grid[x, temp_j - 1] = grid[x, temp_j];
                        grid[x, temp_j].position += new Vector3(0, -1, 0);
                        grid[x, temp_j] = null;
                        temp_j--;
                    }
                }
            }
        }
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
        // initiate dict
        foreach (Transform child in grid)
        {
            Debug.Log("The new entry is: " + child.name);
            visited[child] = false;
        }

        Debug.Log(visited);

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

        Debug.Log("-------------------------------------------------------");
        Debug.Log(visited);
        Debug.Log("=======================================================");
        // The lowest tile in the adjacent tiles group
        List<Transform> noBaseTile = new List<Transform>();

         
        // Find all tiles in visited that has no tile under it
        foreach (Transform child in visited.Keys)
        {
            if (visited[child] == true)
            {
                Vector2 underChild = new Vector2(child.position.x, child.position.y - 1);
                Transform underTile = grid[(int)underChild.x, (int)underChild.y];
                if (underTile == null)
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
                    if (visited.ContainsKey(grid[x,y]) && !grid[x,y-1]) // And if the gird could fall.
                    {
                        lowestFallDistance -= 1;
                        grid[x, y - 1] = grid[x, y];
                        grid[x, y].position += new Vector3(0, -1, 0);
                        grid[x, y] = null;
                    }
                }
            }
        }
    }

    /* 
     >>> DelayEffect: delay any cancelation for certain seconds.
     Issue remain: static does not accept non static inside.
    */

    public void DelayEffect(string text, float time)
    {
        Debug.Log(text);
        StartCoroutine(DelayForSecond(time));
    }

    IEnumerator DelayForSecond(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void Restart()
    {
        Reset();  
        SceneManager.LoadScene("Main");
        //spawner.SpawnNext();
    }

    public void Quit()
    {
        Reset();
        SceneManager.LoadScene("Start");
    }

    public static void Reset()
    {
        //Debug.Log("Resetting");
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }
        //Data.score = 0.0f;
        Data.fallSpeed = 0.4f;
        Data.goal = 10;
        //scoreText.text = "SCORE:\n" + score.ToString("0.00");
        Timer.timeRemain = 120f;
        plusCountP1 = 0;
        subCountP1 = 0;
        mulCountP1 = 0;
        divCountP1 = 0;
        //Debug.Log(plusCount + "\t" + subCount + "\t" + mulCount + "\t" + divCount);
     //   Spawner.unlocked.GetRange(0, 2);
    }
}
