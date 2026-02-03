using UnityEngine;


[System.Serializable]
public class LevelData
{
    public string levelName;
    public Vector3 gravity;
    public int thrustSpeed;
    public int forwardSpeed;
    public string songID;
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

    public string jsonData;
}
