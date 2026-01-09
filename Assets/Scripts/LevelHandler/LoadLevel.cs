using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Collections.Generic;
public class LoadLevel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject levelParent;
    [Header("Prefabs")]
    public GameObject[] levelObjects;



    Dictionary<string, GameObject> prefabMap;

    private void Awake()
    {


        prefabMap = new Dictionary<string, GameObject>();

        foreach (var obj in levelObjects)
        {
            prefabMap.Add(obj.GetComponent<LevelObject>().type, obj);
        }
       
    }
    void Start()
    {

        string path = Path.Combine(Application.streamingAssetsPath, "Levels/"+LevelSelection.SelectedLevel);

        if (!File.Exists(path))
        {
            Debug.Log("Level file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);

        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        Debug.Log("Loaded level: " + levelData.levelName);
        Debug.Log("Gravity: " + levelData.gravity);
      
        SpawnObjects(levelData);
        

    }

    void SpawnObjects(LevelData levelData)
    {
        Debug.Log(levelData.objects.Length);
        foreach (LevelObjectData obj in levelData.objects)
        {
            Debug.Log("TEST");
            if (!prefabMap.TryGetValue(obj.type, out GameObject prefab))
            {
                Debug.LogWarning("Unknown object type: " + obj.type);
                continue;
            }
            prefabMap.TryGetValue(obj.type, out GameObject objPrefab);
            

            Vector3 position = new Vector3(obj.x, obj.y, obj.z);

            Instantiate(objPrefab,position,Quaternion.identity,levelParent.transform);
        }
    }



}
