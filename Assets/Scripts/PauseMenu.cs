using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenu;
    GameObject musicPlayer;
    AudioSource musicSrc;
    public Button resumeButton;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the music player and its audio source
        musicPlayer = GameObject.FindGameObjectWithTag("Music");
        musicSrc = musicPlayer.GetComponent<AudioSource>();
    }

    public void Pause() 
    {
        // Either pause or unpause appropriately
        if (!isPaused)
        {
            // Stop time, show pause menu, pause music
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            musicSrc.Pause();
            resumeButton.Select();
        }
        else
        {
            // Restart time, hide pause menu, play music
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            musicSrc.Play();
        }
        isPaused = !isPaused;
    }

    // Triggered by a button, resume gameplay
    public void Resume() 
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        musicSrc.Play();
    }
    // Triggered by a button, restart the game from level 1
    public void Restart() 
    {
        Resume();
        Destroy(musicPlayer);
        GameData.level = 1;
        SceneManager.LoadScene(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex);
    }
    // Triggered by a button, go back to the main menu
    public void MainMenu() 
    {
        Resume();
        Destroy(musicPlayer);
        GameData.level = 1;
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
