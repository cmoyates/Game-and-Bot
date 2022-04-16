using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    // References to the instructions and credits pannels respectively
    public GameObject instructions;
    public GameObject credits;
    public GameObject settings;
    public GameObject[] mainUIObjects;
    public AudioMixer mixer;
    public Slider musicSlider;
    public TextMeshProUGUI musicVolumeText;
    public Slider sfxSlider;
    public TextMeshProUGUI sfxVolumeText;
    public Button instructionsBackButton;
    public Button instructionsButton;
    public Button creditsBackButton;
    public Button creditsButton;
    public Button settingsBackButton;
    public Button settingsButton;

    private void Start()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

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
        ToggleMainUI(false);
        instructionsBackButton.Select();
    }
    // Hides the instructions pannel
    public void HideInstructions() 
    {
        instructions.SetActive(false);
        ToggleMainUI(true);
        instructionsButton.Select();
    }
    // Shows the credits pannel
    public void ShowCredits() 
    {
        credits.SetActive(true);
        ToggleMainUI(false);
        creditsBackButton.Select();
    }
    // Hides the credits pannel
    public void HideCredits() 
    {
        credits.SetActive(false);
        ToggleMainUI(true);
        creditsButton.Select();
    }
    // Shows the settings panel
    public void ShowSettings() 
    {
        settings.SetActive(true);
        ToggleMainUI(false);
        settingsBackButton.Select();
    }
    // Hides the settings panel
    public void HideSettings() 
    {
        settings.SetActive(false);
        ToggleMainUI(true);
        settingsButton.Select();
    }

    // Toggles the main UI when panels are shown
    public void ToggleMainUI(bool active) 
    {
        foreach (GameObject item in mainUIObjects)
        {
            item.SetActive(active);
        }
    }

    // Sets the volume for the music
    void SetMusicVolume(float volume) 
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        musicVolumeText.text = "Music Volume: " + Mathf.RoundToInt(volume * 100);
    }
    // Sets the volume for the sound effects
    void SetSFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        sfxVolumeText.text = "SFX Volume: " + Mathf.RoundToInt(volume * 100);
    }
}
