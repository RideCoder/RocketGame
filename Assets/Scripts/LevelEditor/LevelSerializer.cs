using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelSerializer : MonoBehaviour
{
    public Transform levelParent;

    public TMP_InputField inputField;
    public void Save()
    {
        Debug.Log("WHAT");

        LevelData level = new LevelData();
        level.levelName = inputField.text;
        Debug.Log(level.levelName);
        level.gravity = -9.81f;

        List<LevelObjectData> objects = new();

        foreach (Transform child in levelParent)
        {
            LevelObject lo = child.GetComponent<LevelObject>();
            if (!lo) continue;

            Vector3 p = child.position;

            objects.Add(new LevelObjectData
            {
                type = lo.type,
                x = p.x,
                y = p.y,
                z = p.z
            });
        }

        level.objects = objects.ToArray();
            string json = JsonUtility.ToJson(level, true);
        
        string path = Path.Combine(Application.streamingAssetsPath, "Levels", inputField.text + ".json");
        File.WriteAllText(path, json);
        Debug.Log("Saved level to " + path);

    }
    }
