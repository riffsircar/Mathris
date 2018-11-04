﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    // Different shapes of blocks
    public GameObject[] shapes;
    public GameObject[] numbers;
    public GameObject[] operators;
    string[] shape = { "I", "J", "L", "O", "S", "T", "Z" };
    
    static Dictionary<string, int[]> X;
    static Dictionary<string, int[]> Y;

    private const float OP_PROB = 0.4f;
    private const float FIRST_PROB = 0.2f;

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
        InitXY();
        SpawnNext();
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

    }

    void BuildNextShape(string shape)
    {
        int[] x = X[shape];
        int[] y = Y[shape];
        
        GameObject block = new GameObject(shape);
        block.transform.position = this.transform.position;
        block.AddComponent<MoveShape>();
        
        float op = Random.Range(0.0f, 1.0f);

        for(int i=0; i < 4; i++)
        {
            int num = Random.Range(0, 10);
            GameObject tile;
            if ((op <= FIRST_PROB && i == 0) || (op > FIRST_PROB && op <= OP_PROB && i == 3))
                tile = Instantiate(operators[Random.Range(0, 4)]);
            else
                tile = Instantiate(numbers[num]);

            tile.transform.parent = block.transform;
            tile.transform.localPosition = new Vector3(x[i], y[i], 0);
        }
        
    }
}
