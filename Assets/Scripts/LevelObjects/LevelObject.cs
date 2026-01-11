using UnityEngine;

public class LevelObject : MonoBehaviour
{
    public string type; // "Block"

    public virtual string SerializeExtraData()
    {
        return null;
    }

    public virtual void DeserializeExtraData(string json) { }

    
}
