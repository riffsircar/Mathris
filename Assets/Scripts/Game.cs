using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    public static int width = 10;
    public static int height = 20;
    static Text scoreText;
    static Text opText;
    static Text goalText;
    public static float score = 0.0f;
    public static float goal;
    public static float fallSpeed = 0.4f;
    static int thresh = 50;
    GameObject scoreObj;
    GameObject goalObj;
    GameObject opObj;
    GameObject spawnerObj;
    GameObject sliderObj;
    static Spawner spawner;
    Spawner spawner2;
    static string operationText;
    public static int plusCount = 0;
    public static int subCount = 0;
    public static int mulCount = 0;
    public static int divCount = 0;
    public static int scoreIncrement = 50;

    // player has freedom to choose if they need a tutorial
    // call camera
    public static ZoomController mainCamera;
    // booleans for track the first time calculation directions 
    // 0 = false, 1 = true
    //  left to right
    public static int addLeftRight = 0;
    public static int minusLeftRight = 0;
    public static int timesLeftRight = 0;
    public static int dividedLeftRight = 0;
    //  up to down
    public static int addUpDown = 0; 
    public static int minusUpDown = 0;
    public static int timesUpDown = 0;
    public static int dividedUpDown = 0;
    //  four directions
    public static int addFour = 0;
    public static int minusFour = 0;
    public static int timesFour = 0;
    public static int dividedFour = 0;

    public static Game game; 

    static Slider scoreSlider;

    private void Awake()
    {
        game = this;
    }

    void Start()
    {
        scoreObj = GameObject.FindGameObjectWithTag("Score");
        goalObj = GameObject.FindGameObjectWithTag("Goal");
        opObj = GameObject.FindGameObjectWithTag("Operation");
        sliderObj = GameObject.Find("ScoreSlider");
        if(sliderObj)
            scoreSlider = sliderObj.GetComponent<Slider>();

        mainCamera = GameObject.Find("Main Camera").GetComponent<ZoomController>();

        if (scoreObj)
        {
            Data.score = 0.0f;
            score = 0.0f;
            scoreSlider.value = score;
            Data.fallSpeed = 0.4f;
            goal = Data.goal;
            //Data.goal = 50;
            scoreText = scoreObj.GetComponent<Text>();
            scoreText.text = "SCORE: 0.00";
            goalText = goalObj.GetComponent<Text>();
            goalText.text = goal.ToString();
            opText = opObj.GetComponent<Text>();
        }
            spawnerObj = GameObject.Find("Spawner");
                if (spawnerObj)
        {
            spawner = spawnerObj.GetComponent<Spawner>();
        }

        Reset();
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
        Debug.Log("DeleteFullRows called");
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
                                operationText = "(" + val_l.ToString() + " " + tile.value + " " + val_r.ToString() + ") + (" + val_u.ToString() + " " + tile.value + " " + val_d.ToString() + ")\n = " + result.ToString("0.00") + "\nDOUBLE OP!";
                                opText.text = "OPERATION: \n" + operationText;

                                Debug.Log("+++:" + tile.value);

                                if (TrackFirstTimeCalculation(tile.value, "all") == 0)
                                {
                                    mainCamera.ChangeCamera(new Vector3(x, y, -20f));
                                    Game.ChangeFirstTimeCalculation(tile.value, "all"); 
                                }

                                SlowDestroyWithEffect(tile, x, y, "all", t, result);

                                //AdjustRows(x, y);
                                //Fix(x, y, t);
                                //UpdateScore(result*2.0f);
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
                                    operationText = "(" + val_l.ToString() + " " + tile.value + " " + val_r.ToString() + ") = " + result.ToString("0.00");
                                    //Debug.Log("OP: " + operationText);
                                    opText.text = "OPERATION: \n" + operationText;

                                    Debug.Log("+++:" + tile.value);

                                    if (TrackFirstTimeCalculation(tile.value, "lr") == 0)
                                    {
                                        mainCamera.ChangeCamera(new Vector3(x, y, -20f));
                                        Game.ChangeFirstTimeCalculation(tile.value, "lr");
                                    }

                                    SlowDestroyWithEffect(tile, x, y, "lr", t, result);

                                    //AdjustRows(x, y);
                                    //Fix(x, y, t);
                                    ////AdjustTest(x, y + 1);
                                    //UpdateScore(result);
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
                                        operationText = "(" + val_u.ToString() + " " + tile.value + " " + val_d.ToString() + ") = " + result.ToString("0.00");
                                        //Debug.Log("OP: " + operationText);
                                        opText.text = "OPERATION: \n" + operationText;

                                        Debug.Log("+++:" + tile.value);

                                        if (TrackFirstTimeCalculation(tile.value, "ud") == 0)
                                        {
                                            mainCamera.ChangeCamera(new Vector3(x, y, -20f));
                                            ChangeFirstTimeCalculation(tile.value, "ud");
                                        }

                                        SlowDestroyWithEffect(tile, x, y, "ud", t, result);

                                        //AdjustTest(x, y + 2);
                                        //Fix(x, y, t);
                                        //UpdateScore(result);
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
        score += res;
        scoreSlider.value = score;
        //scoreText.text = "SCORE:\n" + Data.score.ToString("0.00");
        scoreText.text = "SCORE: " + score.ToString("0.00");
        // When player reach the goal
        //if (Data.score >= Data.goal)
        if(score >= goal) 
        {
            // Change the speed
            Data.fallSpeed /= 1.025f;
            //Debug.Log(Data.fallSpeed);
            //Debug.Log("FASTER!");

            // Change the goal and time
            goal += scoreIncrement;
            goalText.text = goal.ToString();
            scoreSlider.minValue = score;
            scoreSlider.maxValue = goal;
            Timer.timeRemain += 30f;
        }
    }

    static float CalculateResult(int a, int b, string op)
    {
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
        Spawner.UpdateUnlockedOperators();
        return result;
    }

    public static void DestroyWithParticleEffect(Vector3 tilePosition, GameObject particleEffect)
    {
        //Destroy(tile.gameObject);
        Vector3 adjust = new Vector3(0, 0, -15);
        GameObject particleSystem = Instantiate(particleEffect, tilePosition + adjust, Quaternion.identity);
        Destroy(particleSystem, 0.5f);
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
            if(grid[col + 1,y] != null)
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
     >>> DelayEffect: delay following thing for certain seconds:
                > cancelation (destroy)    
                > pause the spawner
                > pause the timer               
    */

    IEnumerator DelayForSecond(float time, Tile tile, int x, int y, string direction, Transform t, float result)
    {
        Debug.Log("Delay for second");
        // handle the color change effects
        Tile number1;
        Tile op;
        Tile number2;
        Tile number3;
        Tile number4;

        if (direction == "lr")
        {
            number1 = grid[x - 1, y].gameObject.GetComponent<Tile>();
            op = grid[x, y].gameObject.GetComponent<Tile>();
            number2 = grid[x + 1, y].gameObject.GetComponent<Tile>();

            number1.GetComponent<SpriteRenderer>().sprite = number1.constrast;
            op.GetComponent<SpriteRenderer>().sprite = op.constrast;
            number2.GetComponent<SpriteRenderer>().sprite = number2.constrast;

            yield return new WaitForSeconds(time);
            Destroy(grid[x, y].gameObject);
            DestroyWithParticleEffect(grid[x, y].position, tile.particleEffect);
            grid[x, y] = null;
            Debug.Log("X: " + x + "\tY: " + y);
            Debug.Log(grid[x - 1, y]);
            if (grid[x-1,y])
                Destroy(grid[x - 1, y].gameObject);
            grid[x - 1, y] = null;

            if(grid[x+1,y])
                Destroy(grid[x + 1, y].gameObject);
            grid[x + 1, y] = null;

            AdjustRows(x, y);
            Fix(x, y, t);
            UpdateScore(result);
        }
        else if (direction == "ud")
        {
            number1 = grid[x, y - 1].gameObject.GetComponent<Tile>();
            op = grid[x, y].gameObject.GetComponent<Tile>();
            number2 = grid[x, y + 1].gameObject.GetComponent<Tile>();

            number1.GetComponent<SpriteRenderer>().sprite = number1.constrast;
            op.GetComponent<SpriteRenderer>().sprite = op.constrast;
            number2.GetComponent<SpriteRenderer>().sprite = number2.constrast;

            yield return new WaitForSeconds(time);

            Destroy(grid[x, y].gameObject);
            DestroyWithParticleEffect(grid[x, y].position, tile.particleEffect);
            grid[x, y] = null;
            Destroy(grid[x, y - 1].gameObject);
            grid[x, y - 1] = null;
            Destroy(grid[x, y + 1].gameObject);
            grid[x, y + 1] = null;

            //AdjustRows(x, y + 2);
            Fix(x, y, t);
            UpdateScore(result);
        }
        else if (direction == "all")
        {
            number1 = grid[x - 1, y].gameObject.GetComponent<Tile>();
            op = grid[x, y].gameObject.GetComponent<Tile>();
            number2 = grid[x + 1, y].gameObject.GetComponent<Tile>();
            number3 = grid[x, y - 1].gameObject.GetComponent<Tile>();
            number4 = grid[x, y + 1].gameObject.GetComponent<Tile>();

            number1.GetComponent<SpriteRenderer>().sprite = number1.constrast;
            op.GetComponent<SpriteRenderer>().sprite = op.constrast;
            number2.GetComponent<SpriteRenderer>().sprite = number2.constrast;
            number3.GetComponent<SpriteRenderer>().sprite = number3.constrast;
            number4.GetComponent<SpriteRenderer>().sprite = number4.constrast;

            yield return new WaitForSeconds(time);

            Destroy(grid[x, y].gameObject);
            DestroyWithParticleEffect(grid[x, y].position, tile.particleEffect);
            grid[x, y] = null;

            Destroy(grid[x - 1, y].gameObject);
            grid[x - 1, y] = null;
            Destroy(grid[x + 1, y].gameObject);
            grid[x + 1, y] = null;

            Destroy(grid[x, y - 1].gameObject);
            grid[x, y - 1] = null;
            Destroy(grid[x, y + 1].gameObject);
            grid[x, y + 1] = null;

            AdjustRows(x, y);
            Fix(x, y, t);
            UpdateScore(result * 2.0f);
        }
    }

    // Track all the fir time calculation that the player make in the game
    // the directions strings are:
    //      1. lr (left to right)
    //      2. ud (up to down)
    //      3. all (all four direction)
    static int TrackFirstTimeCalculation(string op, string direction)
    {
        if (op == "+")
        {
            if (direction == "lr")
            {
                return addLeftRight;
            }
            else if (direction == "ud")
            {
                return addUpDown;
            }
            else if (direction == "all")
            {
                return addFour;
            }
        }
        else if (op == "-")
        {
            if (direction == "lr")
            {
                return minusLeftRight;
            }
            else if (direction == "ud")
            {
                return minusUpDown;
            }
            else if (direction == "all")
            {
                return minusFour;
            }
        }
        else if (op == "*")
        {
            if (direction == "lr")
            {
                return timesLeftRight;
            }
            else if (direction == "ud")
            {
                return timesUpDown;
            }
            else if (direction == "all")
            {
                return timesFour;
            }
        }
        else if (op == "/")
        {
            if (direction == "lr")
            {
                return dividedLeftRight;
            }
            else if (direction == "ud")
            {
                return dividedUpDown;
            }
            else if (direction == "all")
            {
                return dividedFour;
            }
        }
        return -1;
    }

    // Change corresponding bool value
    static void ChangeFirstTimeCalculation(string op, string direction)
    {
        if (op == "+")
        {
            if(direction == "lr")
            {
                //Debug.Log("Changed addLeftRight: " + addLeftRight);
                addLeftRight++;
            }
            else if (direction == "ud")
            {
                //Debug.Log("Changed addUpDown: " + addUpDown);
                addUpDown++;

            }
            else if (direction == "all")
            {
                addFour++;
            }
        }
        else if (op == "-")
        {
            if (direction == "lr")
            {
                minusLeftRight++;
            }
            else if (direction == "ud")
            {
                minusUpDown++;
            }
            else if (direction == "all")
            {
                minusFour++;
            }
        }
        else if (op == "*")
        {
            if (direction == "lr")
            {
                timesLeftRight++;
            }
            else if (direction == "ud")
            {
                timesUpDown++;
            }
            else if (direction == "all")
            {
                timesFour++;
            }
        }
        else if (op == "/")
        {
            if (direction == "lr")
            {
                dividedLeftRight++;
            }
            else if (direction == "ud")
            {
                dividedUpDown++;
            }
            else if (direction == "all")
            {
                dividedFour++;
            }
        }
    }

    // Distroy the block after changing the color
    static void SlowDestroyWithEffect(Tile tile, int x, int y, string direction, Transform t, float result)
    {
        //spawner.isPause = true;
       game.StartCoroutine(game.DelayForSecond(0f, tile, x, y, direction, t, result));
        //spawner.isPause = false;
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
        Debug.Log("Resetting");
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
        goal = Data.goal;
        //scoreText.text = "SCORE:\n" + score.ToString("0.00");
        Timer.timeRemain = Data.initTime;
        plusCount = 0;
        subCount = 0;
        mulCount = 0;
        divCount = 0;
        //Debug.Log(plusCount + "\t" + subCount + "\t" + mulCount + "\t" + divCount);
     //   Spawner.unlocked.GetRange(0, 2);
    }
}
