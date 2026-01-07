using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;
    public Slider soundSlider;
    public Slider musicSlider;
    int currentResolutionIndex = 0;
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
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

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();


    }

    public void SetResolution(int index)
    {
        currentResolutionIndex = index;
    }

    public void SetAudio()
    {

    }
    public void ApplySettings()
    {
        Resolution resolution = resolutions[currentResolutionIndex];

        Screen.SetResolution(resolution.width,resolution.height, Screen.fullScreen);
    }
}
