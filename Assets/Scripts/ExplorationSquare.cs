using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationSquare : MonoBehaviour
{
    // A reference to an AI player controller
    public AIPlayerController ai;

    // If the AI player sees this gameobject, remove it from the AI's list of unexplored locations and destroy itself
    private void OnBecameVisible()
    {
        ai.explSquares.Remove(transform);
        Destroy(gameObject);
    }
}
