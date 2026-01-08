using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelSerializer : MonoBehaviour
{
    public Transform levelParent;


    public void Save(string fileName)
    {

        LevelData level = new LevelData();
        level.levelName = "Custom Level";
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
        string path = Path.Combine(Application.streamingAssetsPath, "Levels", fileName+".json");
        File.WriteAllText(path, json);
        Debug.Log("Saved level to " + path);

    }
    }
