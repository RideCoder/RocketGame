using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LevelSerializer : MonoBehaviour
{
    public Transform levelParent;
    public TMP_InputField inputField;
    public string levelName;

    /// <summary>
    /// Save the current level to persistentDataPath (works in WebGL)
    /// </summary>
    public void Save()
    {
        LevelData level = new LevelData();
        level.levelName = inputField.text;
        levelName = level.levelName;
        level.gravity = -9.81f;

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
        level.gravity = -9.81f;

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
