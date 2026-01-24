using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    Resolution[] resolutions;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider soundSlider;
    public Slider musicSlider;
    public Slider masterSlider;

    int currentResolutionIndex = 0;

    public static float MasterVolume = 1f;



    //Boost Button
    public static List<ButtonControl> buttons;
    public static int boostInputIndex = 0;
    public TMP_Text boostText;

    //Resolution Option
    public List<string> options;
    public TMP_Text resolutionText;

    //FPS 
    public List<int> fpsList;
    public TMP_Text fpsText;
    public static int fpsIndex;



    private void Start()
    {
        // ---------- AUDIO INIT ----------
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterSlider.value = master;
        musicSlider.value = music;
        soundSlider.value = sfx;

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSoundVolume(sfx);

        // ---------- INPUT INIT ----------
        buttons = new List<ButtonControl>
        {
            Mouse.current.leftButton,
            Keyboard.current.upArrowKey,
            Keyboard.current.spaceKey
        };

        

        UpdateBoostText();

        // ---------- RESOLUTION INIT ----------
        resolutions = Screen.resolutions;
        options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionText.text = "Resolution: " + options[currentResolutionIndex];
        fpsText.text = "Frame Rate: " + fpsList[fpsIndex] + " FPS";
        
    }

    // ---------- AUDIO ----------
    private float ToDb(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }
    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        audioMixer.SetFloat("MasterVolume", ToDb(value));
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", ToDb(value));
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSoundVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", ToDb(value));
        PlayerPrefs.SetFloat("SFXVolume", value);
    }


    // ---------- RESOLUTION ----------
    public void SetResolution()
    {
        currentResolutionIndex++;
        if (currentResolutionIndex >= resolutions.Length)
        {
            currentResolutionIndex = 0;
        }

        resolutionText.text = "Resolution: " + options[currentResolutionIndex];
    }

  
    // ---------- FPS ----------
    public void ChangeFPS()
    {
        fpsIndex++;
        if (fpsIndex >= fpsList.Count)
        {
            fpsIndex = 0;
        }

        fpsText.text = "Frame Rate: " + fpsList[fpsIndex] + " FPS";
    }

 
    // ---------- INPUT ----------
    public void ChangeInput()
    {
        boostInputIndex++;
        if (boostInputIndex >= buttons.Count)
        {
            boostInputIndex = 0;
        }

        UpdateBoostText();
    }

    private void UpdateBoostText()
    {
        if (boostInputIndex == 0)
            boostText.text = "Boost: Left Mouse";
        else if (boostInputIndex == 1)
            boostText.text = "Boost: Up Arrow";
        else if (boostInputIndex == 2)
            boostText.text = "Boost: Space";
    }

    // ---------- APPLY ----------
    public void ApplySettings()
    {
        Resolution resolution = resolutions[currentResolutionIndex];
        Application.targetFrameRate = fpsList[fpsIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
