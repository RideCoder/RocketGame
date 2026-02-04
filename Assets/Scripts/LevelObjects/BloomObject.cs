using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomObject : LevelObject
{
    public float bloomIntensity;
    public override string SerializeExtraData()
    {
        BloomObjectData data = new BloomObjectData
        {
            intensity = bloomIntensity

        };




        return JsonUtility.ToJson(data);
    }

    public override void DeserializeExtraData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        BloomObjectData data = JsonUtility.FromJson<BloomObjectData>(json);
        bloomIntensity = data.intensity;
        
    }

   



    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent ||
            !other.transform.parent.TryGetComponent<PlayerController>(out _))
            return;

        VolumeManager.Instance.SetBloom(bloomIntensity);



    }

    // Optional: stop rainbow when leaving trigger
    [System.Serializable]
    public class BloomObjectData
    {
        public float intensity;
    }
}
