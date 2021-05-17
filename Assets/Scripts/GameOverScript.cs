using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public Text levelText;

    // Start is called before the first frame update
    void Start()
    {
        // Change the text displayed on the GameOver screen depending on whether or not the player is a bot
        string playerName = (GameData.previousScene == 2) ? "The bot" : "You";
        levelText.text = playerName + " made it to level: " + GameData.level;
    }

    // Function triggered by a UI button that restarts the game
    public void Restart() 
    {
        GameData.level = 1;
        SceneManager.LoadScene(sceneBuildIndex: GameData.previousScene);
    }

    // Function triggered by a button in the UI that returns to the main menu
    public void ToMainMenu() 
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
