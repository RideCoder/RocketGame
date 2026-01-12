using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class URPChange : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume volume;

    [Header("Rainbow Settings")]
    public float rainbowSpeed = 60f;   // degrees per second
    public float bloomIntensity = 2f;
    public float saturation = 50f;

    ColorAdjustments colorAdjustments;
    Bloom bloom;

    bool rainbowActive;

    void Awake()
    {
        if (volume.profile == null)
            volume.profile = Instantiate(volume.sharedProfile);

        if (!volume.profile.TryGet(out colorAdjustments))
            colorAdjustments = volume.profile.Add<ColorAdjustments>(true);

        if (!volume.profile.TryGet(out bloom))
            bloom = volume.profile.Add<Bloom>(true);

        colorAdjustments.active = true;
        bloom.active = true;
    }

    void Update()
    {
        if (!rainbowActive) return;

        // Advance hue forever
        float hue = colorAdjustments.hueShift.value;
        hue += rainbowSpeed * Time.deltaTime;

        // Wrap into valid URP range (-180..180)
        if (hue > 180f) hue -= 360f;

        colorAdjustments.hueShift.value = hue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent ||
            !other.transform.parent.TryGetComponent<PlayerController>(out _))
            return;

        // Activate rainbow
        bloom.intensity.value = bloomIntensity;
        colorAdjustments.saturation.value = saturation;
        rainbowActive = true;
    }

    // Optional: stop rainbow when leaving trigger
    
}
