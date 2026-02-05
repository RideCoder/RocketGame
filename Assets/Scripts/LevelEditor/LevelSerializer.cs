using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LevelSerializer : MonoBehaviour
{
    public Transform levelParent;
    public TMP_InputField inputField;

    public TMP_InputField inputGravityFieldX;
    public TMP_InputField inputGravityFieldY;
    public TMP_InputField inputGravityFieldZ;
    public TMP_InputField songIDField;
    public TMP_InputField forwardSpeedField;
    public TMP_InputField thrustSpeedField;
    public string levelName;

    /// <summary>
    /// Save the current level to persistentDataPath (works in WebGL)
    /// </summary>
    public void Save()
    {
        LevelData level = new LevelData();
        level.levelName = inputField.text;
        levelName = level.levelName;
        Vector3 gravity = new Vector3(0f, -9.81f, 0f);
        if (inputGravityFieldX != null &&
            inputGravityFieldY != null &&
            inputGravityFieldZ != null &&
            float.TryParse(inputGravityFieldX.text, out float gx) &&
            float.TryParse(inputGravityFieldY.text, out float gy) &&
            float.TryParse(inputGravityFieldZ.text, out float gz))
        {
            gravity = new Vector3(gx, gy, gz);
        }

        level.gravity = gravity;
        int speed = 15;
        level.forwardSpeed = speed;

        if (forwardSpeedField != null && int.TryParse(forwardSpeedField.text, out speed))
        {
            level.forwardSpeed = speed;
        }

        int thrust = 30;
        level.thrustSpeed = thrust;
        if (thrustSpeedField != null && int.TryParse(thrustSpeedField.text, out thrust))
        {
            level.thrustSpeed = thrust;
        }
        Debug.Log(level.thrustSpeed);

        List<LevelObjectData> objects = new();

        foreach (Transform child in levelParent)
        {
            LevelObject lo = child.GetComponent<LevelObject>();
            if (!lo) continue;

            Vector3 p = child.position;
            Vector3 scale = child.localScale;
            Vector3 rotation = child.eulerAngles;

            objects.Add(new LevelObjectData
            {
                type = lo.type,
                x = p.x,
                y = p.y,
                z = p.z,
                xScale = scale.x,
                yScale = scale.y,
                zScale = scale.z,
                xRotation = rotation.x,
                yRotation = rotation.y,
                zRotation = rotation.z,
                jsonData = lo.SerializeExtraData()
            });
        }

        level.objects = objects.ToArray();
        string json = JsonUtility.ToJson(level, true);

        // Save to persistent path (WebGL-safe)
        string dir = Path.Combine(Application.persistentDataPath, "Levels");
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, levelName + ".json");
        File.WriteAllText(path, json);

        Debug.Log("Saved level to: " + path);
    }

    /// <summary>
    /// Serialize the level to JSON string directly (no file needed)
    /// </summary>
    public string SerializeToJson()
    {

        LevelData level = new LevelData();
        level.levelName = inputField.text;
        levelName = level.levelName;
        Vector3 gravity = new Vector3(0f, -9.81f, 0f);
        if (inputGravityFieldX != null &&
            inputGravityFieldY != null &&
            inputGravityFieldZ != null &&
            float.TryParse(inputGravityFieldX.text, out float gx) &&
            float.TryParse(inputGravityFieldY.text, out float gy) &&
            float.TryParse(inputGravityFieldZ.text, out float gz))
        {
            gravity = new Vector3(gx, gy, gz);
        }

        level.gravity = gravity;
        int speed = 15;
        level.forwardSpeed = speed;

        if (forwardSpeedField != null && int.TryParse(forwardSpeedField.text, out speed))
        {
            level.forwardSpeed = speed;
        }

        int thrust = 30;
        level.thrustSpeed = thrust;
        if (thrustSpeedField != null && int.TryParse(thrustSpeedField.text, out thrust))
        {
            level.thrustSpeed = thrust;
        }
        Debug.Log(level.thrustSpeed);

        List<LevelObjectData> objects = new();

        foreach (Transform child in levelParent)
        {
            LevelObject lo = child.GetComponent<LevelObject>();
            if (!lo) continue;

            Vector3 p = child.position;
            Vector3 scale = child.localScale;
            Vector3 rotation = child.eulerAngles;

            objects.Add(new LevelObjectData
            {
                type = lo.type,
                x = p.x,
                y = p.y,
                z = p.z,
                xScale = scale.x,
                yScale = scale.y,
                zScale = scale.z,
                xRotation = rotation.x,
                yRotation = rotation.y,
                zRotation = rotation.z,
                jsonData = lo.SerializeExtraData()
            });
        }

        level.objects = objects.ToArray();
        return JsonUtility.ToJson(level, true);
    }
}
