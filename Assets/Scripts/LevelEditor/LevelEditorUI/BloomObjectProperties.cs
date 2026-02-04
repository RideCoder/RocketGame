using UnityEngine;
using UnityEngine.UI;

public class BloomObjectProperties : MonoBehaviour
{
    public Slider bloomIntensitySlider;
 

    private bool ignoreSliderEvents;

    private void OnEnable()
    {
        ignoreSliderEvents = true;

        bool mixedIntensity = false;
     

        float? intensityValue = null;
        

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (!go.TryGetComponent(out BloomObject bloom))
                continue;

            if (intensityValue == null)
                intensityValue = bloom.bloomIntensity;
            else if (intensityValue != bloom.bloomIntensity)
                mixedIntensity = true;

           
        }

        if (!mixedIntensity && intensityValue.HasValue)
            bloomIntensitySlider.value = intensityValue.Value;


        ignoreSliderEvents = false;
    }

    public void OnBloomIntensityChanged(float value)
    {
        if (ignoreSliderEvents)
            return;

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (go.TryGetComponent(out BloomObject bloom))
                bloom.bloomIntensity = value;
        }
        Debug.Log("WTF");
    }

 
}
