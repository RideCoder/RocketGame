using UnityEngine;
using UnityEngine.UI;

public class ColorAdjustmentObjectProperties : MonoBehaviour
{
    public Slider hueShiftSlider;
    public Slider saturationSlider;

    private bool ignoreSliderEvents;

    private void OnEnable()
    {
        ignoreSliderEvents = true;

        bool mixedHueShift = false;
        bool mixedSaturationValue = false;

        int? hueShiftValue = null;
        int? saturationValue = null;

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (!go.TryGetComponent(out ColorAdjustmentObject colorAdjustment))
                continue;

            if (hueShiftValue == null)
                hueShiftValue = colorAdjustment.hueShift;
            else if (hueShiftValue != colorAdjustment.hueShift)
                mixedHueShift = true;

            if (saturationValue == null)
                saturationValue = colorAdjustment.saturationValue;
            else if (saturationValue != colorAdjustment.saturationValue)
                mixedSaturationValue = true;
        }

        if (!mixedHueShift && hueShiftValue.HasValue)
            hueShiftSlider.value = hueShiftValue.Value;

        if (!mixedSaturationValue && saturationValue.HasValue)
            saturationSlider.value = saturationValue.Value;

        ignoreSliderEvents = false;
    }

    public void OnHueShiftChanged(float value)
    {
        if (ignoreSliderEvents)
            return;

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (go.TryGetComponent(out ColorAdjustmentObject colorAdjustment))
                colorAdjustment.hueShift = Mathf.RoundToInt(value);
        }
        Debug.Log("WTF");
    }

    public void OnSaturationChanged(float value)
    {
        if (ignoreSliderEvents)
            return;

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (go.TryGetComponent(out ColorAdjustmentObject colorAdjustment))
                colorAdjustment.saturationValue = Mathf.RoundToInt(value);
        }
    }
}
