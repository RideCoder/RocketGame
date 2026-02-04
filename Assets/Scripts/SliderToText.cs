using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderToText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider slider;
    public TMP_Text text;
    void Start()
    {
        slider.onValueChanged.AddListener(v =>
        {
            text.text = (Mathf.RoundToInt(v*10f)/10f).ToString();
         
        });
    }

   
}
