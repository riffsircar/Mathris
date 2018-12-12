using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour {

    // Use for keeping playing the click sounds
    private void Awake()
    {
        //GameObject[] clickSounds = GameObject.FindGameObjectsWithTag("ClickSound");
        //if (clickSounds.Length > 1)
        //{
        //    Destroy(this.gameObject);
        //}
        DontDestroyOnLoad(this.gameObject);
    }
}
