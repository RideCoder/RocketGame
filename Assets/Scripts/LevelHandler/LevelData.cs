using UnityEngine;


[System.Serializable]
public class LevelData
{
    public string levelName;
    public float gravity;
    public LevelObjectData[] objects;
}

[System.Serializable]
public class LevelObjectData
{
    public string type;
    public float x;
    public float y;
    public float z;
    public float xScale;
    public float yScale;
    public float zScale;
    public float xRotation;
    public float yRotation;
    public float zRotation;
}
