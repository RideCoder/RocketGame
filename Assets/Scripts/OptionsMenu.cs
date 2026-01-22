using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Resolution[] resolutions;
    public Slider soundSlider;
    public Slider musicSlider;
    int currentResolutionIndex = 0;

    public static float MasterVolume = 1f;

    

    //Camera Shake
    public bool cameraShake = true;
    public TMP_Text cameraShakeText;

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


    //Post Processing
    public TMP_Text postProcessingText;
    public static bool postProcessing = true;
    private void Start()
    {
        buttons = new List<ButtonControl>
        {
            Mouse.current.leftButton,
            Keyboard.current.upArrowKey,
            Keyboard.current.spaceKey
        };
        if (boostInputIndex == 0)
        {
            boostText.text = "Boost: Left Mouse";
        }
        else if (boostInputIndex == 1)
        {
            boostText.text = "Boost: Up Arrow";
        }
        else if (boostInputIndex == 2)
        {
            boostText.text = "Boost: Space";
        }
        
        resolutions = Screen.resolutions;
       // resolutionDropdown.ClearOptions();
       
        options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

       // resolutionDropdown.AddOptions(options);
     //   resolutionDropdown.value = currentResolutionIndex;
      //   resolutionDropdown.RefreshShownValue();


    }

    public void SetResolution()
    {
        currentResolutionIndex++;
        if (currentResolutionIndex >= resolutions.Length)
        {
            currentResolutionIndex = 0;
        }

        resolutionText.text = "Resolution: "+options[currentResolutionIndex];
    }
    public void SetPostProcessingEffects()
    {
        postProcessing = !postProcessing;
        if (postProcessing)
        {
            postProcessingText.text = "Post Processing Effects: On";
        }
        else
        {
            postProcessingText.text = "Post Processing Effects: Off";
        }

    }
    public void ChangeFPS()
    {
        fpsIndex++;
        if (fpsIndex >= fpsList.Count)
        {
            fpsIndex = 0;
        }
        fpsText.text = "Frame Rate: " + fpsList[fpsIndex].ToString() + " FPS";
        
    }

 
    public void SetMasterVolume(float sound)
    {
        Debug.Log(sound);
    }
    public void SetSoundVolume(float sound)
    {
        Debug.Log(sound);
    }
    public void SetMusicVolume(float sound)
    {
        Debug.Log(sound);
    }

    public void SetScreenShake()
    {
        cameraShake = !cameraShake;
        if (cameraShake)
        {
            cameraShakeText.text = "Camera Shake: On";
        }
        else
        {
            cameraShakeText.text = "Camera Shake: Off";
        }
        
    }

    public void ChangeInput()
    {
        boostInputIndex++;
        if (boostInputIndex >= buttons.Count)
        {
            boostInputIndex = 0;
        }

        if (boostInputIndex == 0)
        {
            boostText.text = "Boost: Left Mouse";
        }
        else if (boostInputIndex == 1)
        {
            boostText.text = "Boost: Up Arrow";
        }
        else if (boostInputIndex == 2)
        {
            boostText.text = "Boost: Space";
        }

    }
    public void ApplySettings()
    {
        Resolution resolution = resolutions[currentResolutionIndex];
        Application.targetFrameRate = fpsList[fpsIndex];
        Screen.SetResolution(resolution.width,resolution.height, Screen.fullScreen);
    }
}
