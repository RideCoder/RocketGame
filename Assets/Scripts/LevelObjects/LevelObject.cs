using UnityEngine;

public class LevelObject : MonoBehaviour
{
    public string type; // "Block"
    public LayerMask mask;
    public bool SurfaceNormalRotation;
    public string categoryType;
    public void Start()
    {
        mask = gameObject.layer;
    }
    public virtual string SerializeExtraData()
    {
        return null;
    }

    public virtual void DeserializeExtraData(string json) { }

    
}
