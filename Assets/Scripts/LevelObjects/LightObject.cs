using UnityEngine;

public class LightObject : LevelObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Light lightComponent;

    public override string SerializeExtraData()
    {
        LightObjectData data = new LightObjectData
        {
            color = lightComponent.color,
            range = lightComponent.range,
            intensity = lightComponent.intensity
        };



        return JsonUtility.ToJson(data);
    }

    public override void DeserializeExtraData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        LightObjectData data = JsonUtility.FromJson<LightObjectData>(json);

        lightComponent.color = data.color;
        lightComponent.range = data.range;
        lightComponent.intensity = data.intensity;
    }


}
