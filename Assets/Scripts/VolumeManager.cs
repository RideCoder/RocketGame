using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    [SerializeField] private Volume globalVolume;

    private Bloom bloom;
    private ColorAdjustments colorAdjustments;
    private void Awake()
    {
        Instance = this;
        globalVolume.profile.TryGet(out bloom);
        globalVolume.profile.TryGet(out colorAdjustments);
    }

    public void SetBloom(float intensity)
    {
        bloom.intensity.value = intensity;
    }

    public void SetSaturation(int saturation)
    {
        colorAdjustments.saturation.value = saturation;
    }

    public void SetHueShift(int hueShift)
    {
        colorAdjustments.hueShift.value = hueShift;
    }

    
   
}
