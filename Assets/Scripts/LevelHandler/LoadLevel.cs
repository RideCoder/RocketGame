using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Collections.Generic;
public class LoadLevel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject levelParent;
    public GameObject missingObject;
    [Header("Prefabs")]
    public List<GameObject> levelObjects = new List<GameObject>();
    public string loadLevel;


    Dictionary<string, GameObject> prefabMap;
    public Vector3 spawnPosition;

    private void Awake()
    {

        levelObjects.AddRange(Resources.LoadAll<GameObject>("LevelObjects"));
        prefabMap = new Dictionary<string, GameObject>();

        foreach (var obj in levelObjects)
        {
            prefabMap.Add(obj.GetComponent<LevelObject>().type, obj);
        }
       
    }
    void Start()
    {
        string path;
        if (loadLevel != "")
        {
           
            path = Path.Combine(Application.streamingAssetsPath, "Levels/" + loadLevel + ".json");
        }
        else
        {
            Debug.Log(LevelSelection.SelectedLevel);
            path = Path.Combine(Application.streamingAssetsPath, "Levels/" + LevelSelection.SelectedLevel);
        }


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
        
        foreach (LevelObjectData obj in levelData.objects)
        {
            if (obj.type == "Spawn")
            {
                spawnPosition = new Vector3(obj.x,obj.y, obj.z);
            }
            
            if (!prefabMap.TryGetValue(obj.type, out GameObject prefab))
            {
                
                GameObject missing = Instantiate(missingObject, new Vector3(obj.x, obj.y, obj.z), Quaternion.identity, levelParent.transform);
                missing.transform.localScale = new Vector3(obj.xScale, obj.yScale, obj.zScale);
                missing.transform.eulerAngles = new Vector3(obj.xRotation, obj.yRotation, obj.zRotation);
              
                Debug.LogWarning("Unknown object type: " + obj.type);
                continue;
            }
            prefabMap.TryGetValue(obj.type, out GameObject objPrefab);
            

            Vector3 position = new Vector3(obj.x, obj.y, obj.z);
            Vector3 scale = new Vector3(obj.xScale, obj.yScale, obj.zScale);
            Vector3 rotation = new Vector3(obj.xRotation, obj.yRotation, obj.zRotation);
            GameObject newObject = Instantiate(objPrefab,position,Quaternion.identity,levelParent.transform);
            newObject.transform.localScale = scale;
            newObject.transform.eulerAngles = rotation;
            LevelObject lo = newObject.GetComponent<LevelObject>();
            if (lo != null)
            {
                lo.DeserializeExtraData(obj.jsonData);
            }
        }
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }



}
