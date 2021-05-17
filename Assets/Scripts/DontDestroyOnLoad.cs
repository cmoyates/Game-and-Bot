using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        // Find all gameobjects with the tag "Music"
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");
        // If there are more than one of them, destroy this object
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        // If this object hasn't been destroyed yet, don't destroy this gameobject when the scene changes
        DontDestroyOnLoad(this.gameObject);
    }
}