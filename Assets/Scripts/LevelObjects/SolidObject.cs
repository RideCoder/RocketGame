using UnityEngine;

public class SolidObject : LevelObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public MeshRenderer meshRenderer;

    public override string SerializeExtraData()
    {
        SolidObjectData data = new SolidObjectData
        {
            color = meshRenderer.materials[0].color,

        };



        return JsonUtility.ToJson(data);
    }

    public override void DeserializeExtraData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        SolidObjectData data = JsonUtility.FromJson<SolidObjectData>(json);

        meshRenderer.materials[0].color = data.color;
      
    }


}
