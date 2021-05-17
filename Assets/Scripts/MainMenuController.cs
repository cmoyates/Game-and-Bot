using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // References to the instructions and credits pannels respectively
    public GameObject instructions;
    public GameObject credits;

    // Triggered by a button, starts a player controlled game
    public void Play() 
    {
        GameData.level = 1;
        GameData.previousScene = 1;
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    // Triggered by a button, starts an AI controlled game
    public void AIPlay() 
    {
        GameData.level = 1;
        GameData.previousScene = 2;
        SceneManager.LoadScene(sceneBuildIndex: 2);
    }

    // Shows the instructions pannel
    public void ShowInstructions() 
    {
        instructions.SetActive(true);
    }
    // Hides the instructions pannel
    public void HideInstructions() 
    {
        instructions.SetActive(false);
    }
    // Shows the credits pannel
    public void ShowCredits() 
    {
        credits.SetActive(true);
    }
    // Hides the credits pannel
    public void HideCredits() 
    {
        credits.SetActive(false);
    }
}
