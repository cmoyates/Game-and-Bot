using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{
    AIPlayerController ai;
    bool isAIGame = false;
    bool seenForFirstTime = false;

    // Start is called before the first frame update
    void Start()
    {
        // If the scene index is the index that uses the AI player remember that it is an IA game and get a reference to the players AI
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            isAIGame = true;
            ai = GameObject.FindGameObjectWithTag("Player").GetComponent<AIPlayerController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player touches the goal increase the level and reload the scene
        if (collision.CompareTag("Player")) 
        {
            GameData.level++;
            SceneManager.LoadScene(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex);
        }
    }

    // If the camera can see the goal
    private void OnBecameVisible()
    {
        // If this is the first time that the goal has been seen and it's and AI game
        if (!seenForFirstTime && isAIGame) 
        {
            // Tell the AI that it knows the location of the goal and tell it the location
            ai.goalPosKnown = true;
            ai.goal = transform;
            seenForFirstTime = true;
        }
    }
}
