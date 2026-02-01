using UnityEngine;
using UnityEngine.UI;

public class SpeedObjectProperties : MonoBehaviour
{
    public Slider forwardSpeedSlider;
    public Slider thrustSpeedSlider;

    private bool ignoreSliderEvents;

    private void OnEnable()
    {
        ignoreSliderEvents = true;

        bool mixedForward = false;
        bool mixedThrust = false;

        int? forwardValue = null;
        int? thrustValue = null;

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (!go.TryGetComponent(out SpeedChangeObject speed))
                continue;

            if (forwardValue == null)
                forwardValue = speed.forwardSpeed;
            else if (forwardValue != speed.forwardSpeed)
                mixedForward = true;

            if (thrustValue == null)
                thrustValue = speed.thrustSpeed;
            else if (thrustValue != speed.thrustSpeed)
                mixedThrust = true;
        }

        if (!mixedForward && forwardValue.HasValue)
            forwardSpeedSlider.value = forwardValue.Value;

        if (!mixedThrust && thrustValue.HasValue)
            thrustSpeedSlider.value = thrustValue.Value;

        ignoreSliderEvents = false;
    }

    public void OnForwardSpeedChanged(float value)
    {
        if (ignoreSliderEvents)
            return;

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (go.TryGetComponent(out SpeedChangeObject speed))
                speed.forwardSpeed = Mathf.RoundToInt(value);
        }
        Debug.Log("WTF");
    }

    public void OnThrustSpeedChanged(float value)
    {
        if (ignoreSliderEvents)
            return;

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (go.TryGetComponent(out SpeedChangeObject speed))
                speed.thrustSpeed = Mathf.RoundToInt(value);
        }
    }
}
