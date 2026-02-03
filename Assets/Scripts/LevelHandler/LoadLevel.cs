using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    public GameObject levelParent;
    public GameObject missingObject;

    [Header("Prefabs")]
    public List<GameObject> levelObjects = new List<GameObject>();
    public string loadLevel;

    private Dictionary<string, GameObject> prefabMap;
    public Vector3 spawnPosition;
    public PlayerController playerController;
 

    private void Awake()
    {
        // Load all prefabs from Resources folder
        levelObjects.AddRange(Resources.LoadAll<GameObject>("LevelObjects"));
        prefabMap = new Dictionary<string, GameObject>();

        foreach (var obj in levelObjects)
        {
            prefabMap.Add(obj.GetComponent<LevelObject>().type, obj);
        }
    }

    private void Start()
    {
        string json = null;

        // 1. Load level JSON from online source if available
        if (LoadOnlineLevels.LevelJson != null)
        {
            json = LoadOnlineLevels.LevelJson;
            LoadOnlineLevels.LevelJson = null;
        }
        else
        {
            // 2. Load level JSON from local persistent storage (WebGL safe)
            string dir = Path.Combine(Application.persistentDataPath, "Levels");
            Directory.CreateDirectory(dir);

            string fileName = !string.IsNullOrEmpty(loadLevel) ? loadLevel + ".json" : LevelSelection.SelectedLevel;
            if (string.IsNullOrEmpty(fileName)) {
                fileName = "";

            }


            string path = Path.Combine(dir, fileName);

            if (!File.Exists(path))
            {
                Debug.LogWarning("Level file not found: " + path);
                return;
            }

            json = File.ReadAllText(path);
        }

        if (!string.IsNullOrEmpty(json))
        {
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            Debug.Log("Loaded level: " + levelData.levelName);
            Debug.Log("Gravity: " + levelData.gravity);
            SpawnObjects(levelData);
            playerController.forwardSpeed = levelData.forwardSpeed;
            playerController.thrustForce = levelData.thrustSpeed;
            Physics.gravity = levelData.gravity;
            Debug.Log(levelData.forwardSpeed);
            Debug.Log(levelData.thrustSpeed);
        }
    }

 
       
    
    private void SpawnObjects(LevelData levelData)
    {
        // Clear previous objects
        foreach (Transform obj in levelParent.transform)
        {
            Destroy(obj.gameObject);
        }

        foreach (LevelObjectData obj in levelData.objects)
        {
            if (obj.type == "Spawn")
            {
                spawnPosition = new Vector3(obj.x, obj.y, obj.z);
            }

            if (!prefabMap.TryGetValue(obj.type, out GameObject prefab))
            {
                // Instantiate missing object placeholder
                GameObject missing = Instantiate(missingObject, new Vector3(obj.x, obj.y, obj.z),
                    Quaternion.identity, levelParent.transform);
                missing.transform.localScale = new Vector3(obj.xScale, obj.yScale, obj.zScale);
                missing.transform.eulerAngles = new Vector3(obj.xRotation, obj.yRotation, obj.zRotation);
                Debug.LogWarning("Unknown object type: " + obj.type);
                continue;
            }

            // Instantiate prefab
            GameObject newObj = Instantiate(prefab, new Vector3(obj.x, obj.y, obj.z),
                Quaternion.identity, levelParent.transform);
            newObj.transform.localScale = new Vector3(obj.xScale, obj.yScale, obj.zScale);
            newObj.transform.eulerAngles = new Vector3(obj.xRotation, obj.yRotation, obj.zRotation);

            // Deserialize extra data
            LevelObject lo = newObj.GetComponent<LevelObject>();
            lo?.DeserializeExtraData(obj.jsonData);
        }
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }
}
