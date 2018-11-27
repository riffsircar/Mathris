﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    // Different shapes of blocks
    public GameObject[] shapes;
    public GameObject[] numbers;
    public GameObject[] operators;
    public GameObject emptyTile;
    string[] shape = { "I", "J", "L", "O", "S", "T", "Z" };
    static Dictionary<string, GameObject> opDict;
    public static List<GameObject> unlocked;

    static Dictionary<string, int[]> X;
    static Dictionary<string, int[]> Y;

    /*
    [SerializeField] float OP_PROB = 0.9f;
    [SerializeField] float FIRST_PROB = 0.5f;
    [SerializeField] float NUM_PROB = 0.6f;
    */
    float EMPTY_PROB = 0.1f;
    float FIRST_PROB = 0.25f;
    float LAST_PROB = 0.4f;

    float OP_PROB = 0.6f;
    float NUM_PROB = 0.9f;

    static GameObject mult;
    static GameObject div;
    public static GameObject addCount;
    public static GameObject subCount;
    public static GameObject divCount;
    public static GameObject mulCount;



    // Spawn the next blocks onto the game surface
    public void SpawnNext()
    {
        int randomIndex = Random.Range(0, shape.Length);

        string nextShape = shape[randomIndex];

        BuildNextShape(nextShape);
        //Instantiate(shapes[randomIndex], transform.position, Quaternion.identity);
        //   Instantiate(nextShapeObj, transform.position, Quaternion.identity);
    }

    void Start()
    {
        //Instantiate(test, transform.position, Quaternion.identity);
        unlocked = new List<GameObject>();
        opDict = new Dictionary<string, GameObject>();
        foreach (GameObject go in operators)
        {
            opDict.Add(go.name, go);
        }

        unlocked.Add(opDict["add"]);
        unlocked.Add(opDict["subtract"]);

        mult = GameObject.Find("Mult");
        mult.active = false;
        div = GameObject.Find("Div");
        div.active = false;

        addCount = GameObject.Find("AddCount");
        mulCount = GameObject.Find("MulCount");
        subCount = GameObject.Find("SubCount");
        divCount = GameObject.Find("DivCount");
        mulCount.active = false;
        divCount.active = false;
        addCount.GetComponent<Text>().text = "0";
        subCount.GetComponent<Text>().text = "0";

        InitXY();
        SpawnNext();
    }

    public static void UpdateUnlockedOperators()
    {
        addCount.GetComponent<Text>().text = Game.plusCount.ToString();
        subCount.GetComponent<Text>().text = Game.subCount.ToString();

        if(divCount)
            divCount.GetComponent<Text>().text = Game.divCount.ToString();
        if(mulCount)
            mulCount.GetComponent<Text>().text = Game.mulCount.ToString();

        if (Game.plusCount == 5)
        {
            if (!mult.active)
            {
                Debug.Log("MULTIPLICATION UNLOCKED!");
                unlocked.Add(opDict["multiply"]);
                mult.active = true;
                mulCount.active = true;
                mulCount.GetComponent<Text>().text = "0";
            }
        }
        if (Game.subCount == 5)
        {
            if (!div.active)
            {
                Debug.Log("DIVISION UNLOCKED!");
                unlocked.Add(opDict["divide"]);
                div.active = true;
                divCount.active = true;
                divCount.GetComponent<Text>().text = "0";
            }
        }
    }

    static void InitXY()
    {
        X = new Dictionary<string, int[]>();
        Y = new Dictionary<string, int[]>();
        X.Add("I", new int[] { 0, 0, 0, 0 });
        X.Add("J", new int[] { 0, 1, 1, 1 });
        X.Add("L", new int[] { 0, 0, 0, 1 });
        X.Add("S", new int[] { 2, 1, 1, 0 });
        X.Add("O", new int[] { 0, 1, 0, 1 });
        X.Add("T", new int[] { 0, 1, 1, 2 });
        X.Add("Z", new int[] { 0, 1, 1, 2 });

        Y.Add("I", new int[] { 0, 1, 2, 3 });
        Y.Add("J", new int[] { 0, 0, 1, 2 });
        Y.Add("L", new int[] { 2, 1, 0, 0 });
        Y.Add("S", new int[] { 1, 1, 0, 0 });
        Y.Add("O", new int[] { 1, 1, 0, 0 });
        Y.Add("T", new int[] { 1, 1, 0, 1 });
        Y.Add("Z", new int[] { 1, 1, 0, 0 });

        // test shape Y: for testing 4 direction calculation
        //X.Add("Y", new int[] { 0, 1, 1, 2 });
        //Y.Add("Y", new int[] { 2, 0, 1, 2 });
    }

    void BuildNextShape(string shape)
    {
        int[] x = X[shape];
        int[] y = Y[shape];

        GameObject block = new GameObject(shape);
        block.transform.position = this.transform.position;
        block.AddComponent<MoveShape>();

        
        float op = Random.Range(0.0f, 1.0f);
        bool hasOp = false;
        int unlockedCount = unlocked.Count;
        Debug.Log("Unlocked Count: " + unlockedCount);
        for (int i = 0; i < 4; i++)
        {
            int num = Random.Range(0, 10);
            GameObject tile;
            if ((i == 0 || i == 3) && !hasOp && op <= OP_PROB)
            {
                    tile = Instantiate(unlocked[Random.Range(0, unlockedCount)]);
                    hasOp = true;
            }
            else
            {
                float tp = Random.Range(0.0f, 1.0f);
                if(tp <= NUM_PROB)
                    tile = Instantiate(numbers[num]);
                else
                    tile = Instantiate(emptyTile);
            }
            
        tile.transform.parent = block.transform;
        tile.transform.localPosition = new Vector3(x[i], y[i], 0);
        }
    }
}
