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
    public static float score1 = 0.0f;
    public static float score2 = 0.0f;
    public static float fallSpeed = 0.4f;
    static int thresh = 50;
    GameObject scoreObj1;
    GameObject scoreObj2;
    GameObject opObj1;
    GameObject opObj2;
    GameObject spawnerObj1;
    GameObject spawnerObj2;
    Spawner2P spawner1;
    Spawner2P spawner2;
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
    public static List<GameObject> unlockedP1;
    public static List<GameObject> unlockedP2;
    static Dictionary<string, GameObject> opDict;
    public static int unlockedP1Count = 2;
    public static int unlockedP2Count = 2;
    public static int winner = 0;

    static GameObject mult1;
    static GameObject div1;
    static GameObject mult2;
    static GameObject div2;

    public static GameObject addCountObjP1;
    public static GameObject subCountObjP1;
    public static GameObject divCountObjP1;
    public static GameObject mulCountObjP1;

    public static GameObject addCountObjP2;
    public static GameObject subCountObjP2;
    public static GameObject divCountObjP2;
    public static GameObject mulCountObjP2;

    

    

    // grid: tracks all position of the grid
    public static Transform[,] grid = new Transform[width, height];
    public static Transform[,] grid1 = new Transform[width, height];
    public static Transform[,] grid2 = new Transform[width, height];

    void Awake()
    {
        scoreObj1 = GameObject.FindGameObjectWithTag("ScoreP1");
        scoreObj2 = GameObject.FindGameObjectWithTag("ScoreP2");
        opObj1 = GameObject.FindGameObjectWithTag("OperationP1");
        opObj2 = GameObject.FindGameObjectWithTag("OperationP2");
        unlockedP1 = new List<GameObject>();
        unlockedP2 = new List<GameObject>();
        if (scoreObj1 && scoreObj2)
        {
            Data.fallSpeed = 0.4f;
            scoreText1 = scoreObj1.GetComponent<Text>();
            scoreText1.text = "SCORE:\n0.00";
            scoreText2 = scoreObj2.GetComponent<Text>();
            scoreText2.text = "SCORE:\n0.00";
            opText1 = opObj1.GetComponent<Text>();
            opText2 = opObj2.GetComponent<Text>();
        }
        
        spawnerObj1 = GameObject.Find("SpawnerP1");
        spawnerObj2 = GameObject.Find("SpawnerP2");
        if (spawnerObj1)
        {
            spawner1 = spawnerObj1.GetComponent<Spawner2P>();
        }
        if (spawnerObj2)
        {
            spawner2 = spawnerObj2.GetComponent<Spawner2P>();
        }
        opDict = new Dictionary<string, GameObject>();
        foreach (GameObject go in spawner1.operators)
        {
            opDict.Add(go.name, go);
        }
        unlockedP1.Add(opDict["add"]);
        unlockedP1.Add(opDict["subtract"]);
        unlockedP2.Add(opDict["add"]);
        unlockedP2.Add(opDict["subtract"]);

        mult1 = GameObject.Find("Mult");
        if (mult1)
            mult1.active = false;
        div1 = GameObject.Find("Div");
        if (div1)
            div1.active = false;

        mult2 = GameObject.Find("MultP2");
        if (mult2)
            mult2.active = false;
        div2 = GameObject.Find("DivP2");
        if (div2)
            div2.active = false;

        addCountObjP1 = GameObject.Find("AddCountP1");
        mulCountObjP1 = GameObject.Find("MulCountP1");
        subCountObjP1 = GameObject.Find("SubCountP1");
        divCountObjP1 = GameObject.Find("DivCountP1");
        if (mulCountObjP1)
            mulCountObjP1.active = false;
        if (divCountObjP1)
            divCountObjP1.active = false;
        addCountObjP1.GetComponent<Text>().text = "0";
        subCountObjP1.GetComponent<Text>().text = "0";

        addCountObjP2 = GameObject.Find("AddCountP2");
        mulCountObjP2 = GameObject.Find("MulCountP2");
        subCountObjP2 = GameObject.Find("SubCountP2");
        divCountObjP2 = GameObject.Find("DivCountP2");
        if (mulCountObjP2)
            mulCountObjP2.active = false;
        if (divCountObjP2)
            divCountObjP2.active = false;
        addCountObjP2.GetComponent<Text>().text = "0";
        subCountObjP2.GetComponent<Text>().text = "0";

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
            return ((int)position.x >= 0 &&
                    (int)position.x < width &&
                    (int)position.y >= 0);
    }

    public static bool InsideGridOneBorder(Vector2 position)
    {
        return ((int)position.x >= 0 &&
                (int)position.x < width &&
                (int)position.y >= 0);
    }

    public static bool InsideGridTwoBorder(Vector2 position)
    {
        return ((int)position.x >= 14 &&
                (int)position.x < (14+width) &&
                (int)position.y >= 0);
    }

    public static bool IsRowFull(int y, int player)
    {
        for (int x = 0; x < width; ++x)
        {
            if (player == 1)
            {
                if (grid1[x, y] == null)
                {
                    return false;
                }
            }
            else
            {
                if (grid2[x, y] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static void DeleteRow(int y, int player)
    {
        for (int x = 0; x < width; x++)
        {
            if (player == 1)
            {
                Destroy(grid1[x, y].gameObject);
                grid1[x, y] = null;
            }
            else
            {
                Destroy(grid2[x, y].gameObject);
                grid2[x, y] = null;
            }
        }
    }

    public static void DeleteFullRows(int player)
    {
        int lines = 0;
        for (int y = 0; y < height; ++y)
        {
            if(IsRowFull(y,player))
            {
                DeleteRow(y, player);
                DecreaseRowsAbove(y + 1, player);
                // reset the y after delete the ro in order to keep the index right
                --y;
                lines++;
            }
        }
        UpdateScore(lines * 5.0f, player);
    }

    public static void DecreaseRow(int y, int player)
    {
        for (int x = 0; x < width; ++x)
        {
            if (player == 1)
            {
                if (grid1[x, y] != null)
                {
                    grid1[x, y - 1] = grid1[x, y];
                    grid1[x, y] = null;
                    // Update Block position
                    grid1[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
            else
            {
                if (grid2[x, y] != null)
                {
                    grid2[x, y - 1] = grid2[x, y];
                    grid2[x, y] = null;
                    // Update Block position
                    grid2[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }
    }
 
    public static void DecreaseRowsAbove(int y, int player)
    {
        for (int i = y; i < height; ++i)
        {
            DecreaseRow(i, player);
        }
    }

    public static void PerformOperations(Transform t, Transform[,] grid, int player)
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
                            
                            float resultLR = CalculateResult(val_l, val_r, tile.value, player);
                            float resultUD = CalculateResult(val_u, val_d, tile.value, player);

                            float result = (resultLR + resultUD); //doubling the result
                            if (result != float.PositiveInfinity)
                            {
                                if (player == 1)
                                {
                                    operationText1 = "(" + val_l.ToString() + " " + tile.value + " " + val_r.ToString() + ") + (" + val_u.ToString() + " " + tile.value + " " + val_d.ToString() + ")\n = " + result.ToString("0.00") + "\nDOUBLE OP!";
                                    opText1.text = "OP: \n" + operationText1;
                                }
                                else
                                {
                                    operationText2 = "(" + val_l.ToString() + " " + tile.value + " " + val_r.ToString() + ") + (" + val_u.ToString() + " " + tile.value + " " + val_d.ToString() + ")\n = " + result.ToString("0.00") + "\nDOUBLE OP!";
                                    opText2.text = "OP: \n" + operationText2;
                                }
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

                                AdjustRows(x, y, grid, player);
                                Fix(x, y, t, grid, player);
                                UpdateScore(result*2.0f, player);
                            }
                            else
                            {
                                Data.cod = "DIVIDE BY ZERO!";
                                if (player == 1)
                                    Game2P.winner = 2;
                                else
                                    Game2P.winner = 1;
                                SceneManager.LoadScene("Over2P");
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
                                float result = CalculateResult(val_l, val_r, tile.value, player);
                                if (result != float.PositiveInfinity)
                                {
                                    if (player == 1)
                                    {
                                        operationText1 = "(" + val_l.ToString() + " " + tile.value + " " + val_r.ToString() + ") = " + result.ToString("0.00");
                                        opText1.text = "OP: \n" + operationText1;
                                    }
                                    else
                                    {
                                        operationText2 = "(" + val_l.ToString() + " " + tile.value + " " + val_r.ToString() + ") = " + result.ToString("0.00");
                                        opText2.text = "OP: \n" + operationText2;
                                    }
                                    Destroy(grid[x, y].gameObject);
                                    DestroyWithParticleEffect(grid[x, y].position, tile.particleEffect);
                                    grid[x, y] = null;
                                    Destroy(grid[x - 1, y].gameObject);
                                    grid[x - 1, y] = null;
                                    Destroy(grid[x + 1, y].gameObject);
                                    grid[x + 1, y] = null;

                                    AdjustRows(x, y, grid, player);
                                    Fix(x, y, t, grid, player);
                                    UpdateScore(result, player);
                                }
                                else
                                {
                                    Data.cod = "DIVIDE BY ZERO!";
                                    if (player == 1)
                                        Game2P.winner = 2;
                                    else
                                        Game2P.winner = 1;
                                    SceneManager.LoadScene("Over2P");
                                }
                            }
                            else
                            {
                                //Up and Down calculation
                                
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
                                    float result = CalculateResult(val_u, val_d, tile.value, player);
                                    if (result != float.PositiveInfinity)
                                    {
                                        if (player == 1)
                                        {
                                            operationText1 = "(" + val_u.ToString() + " " + tile.value + " " + val_d.ToString() + ") = " + result.ToString("0.00");
                                            opText1.text = "OP: \n" + operationText1;
                                        }
                                        else
                                        {
                                            operationText2 = "(" + val_u.ToString() + " " + tile.value + " " + val_d.ToString() + ") = " + result.ToString("0.00");
                                            opText2.text = "OP: \n" + operationText2;
                                        }
                                        Destroy(grid[x, y].gameObject);
                                        DestroyWithParticleEffect(grid[x, y].position, tile.particleEffect);
                                        grid[x, y] = null;
                                        Destroy(grid[x, y - 1].gameObject);
                                        grid[x, y - 1] = null;
                                        Destroy(grid[x, y + 1].gameObject);
                                        grid[x, y + 1] = null;

                                        //AdjustTest(x, y + 2);
                                        Fix(x, y, t, grid, player);
                                        UpdateScore(result, player);
                                    }
                                    else
                                    {
                                        Data.cod = "DIVIDE BY ZERO!";
                                        if (player == 1)
                                            Game2P.winner = 2;
                                        else
                                            Game2P.winner = 1;
                                        SceneManager.LoadScene("Over2P");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    static void UpdateScore(float res, int player)
    {
        if (player == 1)
        {
            score1 += res;
            scoreText1.text = "SCORE:\n" + score1.ToString("0.00");
            
        }
        else
        {
            score2 += res;
            scoreText2.text = "SCORE:\n" + score2.ToString("0.00");
        }
    }

    static float CalculateResult(int a, int b, string op, int player)
    {
        
        float result;
        if(op == "+")
        {
            if (player == 1)
                plusCountP1++;
            else
                plusCountP2++;
            result = (a + b);
        }
        else if(op == "-")
        {
            if (player == 1)
                subCountP1++;
            else
                subCountP2++;
            result = (a - b);
        }
        else if(op == "*")
        {
            if (player == 1)
                mulCountP1++;
            else
                mulCountP2++;
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
                if (player == 1)
                    divCountP1++;
                else
                    divCountP2++;
                result = (a / (float)b);
            }
        }
        UpdateUnlockedOperators();
        return result;
    }

    static void UpdateUnlockedOperators()
    {
        
        addCountObjP1.GetComponent<Text>().text = plusCountP1.ToString();
        subCountObjP1.GetComponent<Text>().text = subCountP1.ToString();

        addCountObjP2.GetComponent<Text>().text = plusCountP2.ToString();
        subCountObjP2.GetComponent<Text>().text = subCountP2.ToString();

        if (divCountObjP1)
            divCountObjP1.GetComponent<Text>().text = divCountP1.ToString();
        if(mulCountObjP1)
            mulCountObjP1.GetComponent<Text>().text = mulCountP1.ToString();

        if (divCountObjP2)
            divCountObjP2.GetComponent<Text>().text = divCountP2.ToString();
        if (mulCountObjP2)
            mulCountObjP2.GetComponent<Text>().text = mulCountP2.ToString();

        if (plusCountP1 == 5)
        {
            if (!mult1.active)
            {
                unlockedP1Count++;
                Debug.Log("MULTIPLICATION UNLOCKED!");
                unlockedP1.Add(opDict["multiply"]);
                mult1.active = true;
                mulCountObjP1.active = true;
                mulCountObjP1.GetComponent<Text>().text = "0";
            }
        }
        if (subCountP1 == 5)
        {
            if (!div1.active)
            {
                unlockedP1Count++;
                Debug.Log("DIVISION UNLOCKED!");
                unlockedP1.Add(opDict["divide"]);
                div1.active = true;
                divCountObjP1.active = true;
                divCountObjP1.GetComponent<Text>().text = "0";
            }
        }

        if (plusCountP2 == 5)
        {
            if (!mult2.active)
            {
                unlockedP2Count++;
                Debug.Log("MULTIPLICATION UNLOCKED!");
                unlockedP2.Add(opDict["multiply"]);
                mult2.active = true;
                mulCountObjP2.active = true;
                mulCountObjP2.GetComponent<Text>().text = "0";
            }
        }
        if (subCountP2 == 5)
        {
            if (!div2.active)
            {
                unlockedP2Count++;
                Debug.Log("DIVISION UNLOCKED!");
                unlockedP2.Add(opDict["divide"]);
                div2.active = true;
                divCountObjP2.active = true;
                divCountObjP2.GetComponent<Text>().text = "0";
            }
        }
    }


    static void DestroyWithParticleEffect(Vector3 tilePosition, GameObject particleEffect)
    {
        //Destroy(tile.gameObject);
        Vector3 adjust = new Vector3(0, 0, -15);
        GameObject particleSystem = Instantiate
            (particleEffect, tilePosition + adjust, Quaternion.identity);
        Destroy(particleSystem, 1f);
    }

    static void AdjustRows(int col, int row, Transform[,] grid, int player)
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

    static void Fix(int col, int row, Transform t, Transform[,] grid, int player)
    {
        List<int> xs = new List<int>();
        List<int> ys = new List<int>();

        foreach(Transform child in t)
        {
            
            int x = (int)Math.Round(child.position.x);
            int y = (int)Math.Round(child.position.y);
            //Debug.Log("X: " + child.position.x + "\t" + "Y: " + child.position.y);
            if (player == 2)
                x -= 14;
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
        SceneManager.LoadScene("TwoPlayer");
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
                if (grid1[x, y] != null)
                {
                    Destroy(grid1[x, y].gameObject);
                    grid1[x, y] = null;
                }

                if (grid2[x, y] != null)
                {
                    Destroy(grid2[x, y].gameObject);
                    grid2[x, y] = null;
                }
            }
        }
        //Data.score = 0.0f;
        Data.fallSpeed = 0.4f;
        //scoreText.text = "SCORE:\n" + score.ToString("0.00");
        Timer.timeRemain = 60f;
        plusCountP1 = 0;
        subCountP1 = 0;
        mulCountP1 = 0;
        divCountP1 = 0;

        plusCountP2 = 0;
        subCountP2 = 0;
        mulCountP2 = 0;
        divCountP2 = 0;

        score1 = 0f;
        score2 = 0f;
        //Debug.Log(plusCount + "\t" + subCount + "\t" + mulCount + "\t" + divCount);
        //   Spawner.unlocked.GetRange(0, 2);
    }
}
