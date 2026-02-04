using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorAdjustmentObject : LevelObject
{
    public int hueShift;
    public int saturationValue;
    public override string SerializeExtraData()
    {
        ColorAdjustmentObjectData data = new ColorAdjustmentObjectData
        {
            hue = hueShift,
            saturation = saturationValue,

        };




        return JsonUtility.ToJson(data);
    }

    public override void DeserializeExtraData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        ColorAdjustmentObjectData data = JsonUtility.FromJson<ColorAdjustmentObjectData>(json);
        hueShift = data.hue;
        saturationValue = data.saturation;  
        
    }

   



    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent ||
            !other.transform.parent.TryGetComponent<PlayerController>(out _))
            return;

        VolumeManager.Instance.SetHueShift(hueShift);
        VolumeManager.Instance.SetSaturation(saturationValue);


    }

    // Optional: stop rainbow when leaving trigger
    [System.Serializable]
    public class ColorAdjustmentObjectData
    {
        public int hue;
        public int saturation;
    }
}
