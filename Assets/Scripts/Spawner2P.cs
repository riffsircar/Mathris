using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner2P : MonoBehaviour
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

    static GameObject mult1;
    static GameObject div1;
    static GameObject mult2;
    static GameObject div2;
    public static GameObject addCountP1;
    public static GameObject subCountP1;
    public static GameObject divCountP1;
    public static GameObject mulCountP1;

    public static GameObject addCountP2;
    public static GameObject subCountP2;
    public static GameObject divCountP2;
    public static GameObject mulCountP2;



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

        mult1 = GameObject.Find("Mult");
        if(mult1)
            mult1.active = false;
        div1 = GameObject.Find("Div");
        if(div1)
            div1.active = false;

        mult2 = GameObject.Find("MultP2");
        if(mult2)
            mult2.active = false;
        div2 = GameObject.Find("DivP2");
        if(div2)
            div2.active = false;

        addCountP1 = GameObject.Find("AddCountP1");
        mulCountP1 = GameObject.Find("MulCountP1");
        subCountP1 = GameObject.Find("SubCountP1");
        divCountP1 = GameObject.Find("DivCountP1");
        if(mulCountP1)
            mulCountP1.active = false;
        if(divCountP1)
            divCountP1.active = false;
        addCountP1.GetComponent<Text>().text = "0";
        subCountP1.GetComponent<Text>().text = "0";

        addCountP2 = GameObject.Find("AddCountP2");
        mulCountP2 = GameObject.Find("MulCountP2");
        subCountP2 = GameObject.Find("SubCountP2");
        divCountP2 = GameObject.Find("DivCountP2");
        if(mulCountP2)
            mulCountP2.active = false;
        if(divCountP2)
            divCountP2.active = false;
        addCountP2.GetComponent<Text>().text = "0";
        subCountP2.GetComponent<Text>().text = "0";

        InitXY();
        SpawnNext();
    }

    public static void UpdateUnlockedOperators()
    {
        /*
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
                Game.scoreIncrement = 20;
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
        */
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
        Debug.Log("GOB NAME: " + gameObject.name);
        if (gameObject.name == "SpawnerP1")
            block.AddComponent<MoveShape>();
        else
            block.AddComponent<MoveShape2P>();

        
        float op = Random.Range(0.0f, 1.0f);
        bool hasOp = false;
        int unlockedCount = unlocked.Count;
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
