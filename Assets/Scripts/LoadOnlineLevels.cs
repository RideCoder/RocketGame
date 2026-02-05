using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOnlineLevels : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject levelsList;
    public GameObject button;
    public static string LevelJson;
    public TMP_InputField inputField;
    public string searchField;
    public Button search;
    public Button nextPage;
    public Button previousPage;
    public List<GameObject> levels = new List<GameObject>();
    public void GetLevels()
    {
        LootLockerSDKManager.GetAssetNextList(10, (listResponse) =>
        {
            if (!listResponse.success)
            {
                Debug.LogError("Failed to get asset list");
                return;
            }

            foreach (var asset in listResponse.assets)
            {
                Debug.Log($"Asset name: {asset.name}");
                Debug.Log($"Asset id: {asset.id}");


                GameObject clone = Instantiate(button);

                clone.transform.parent = levelsList.transform;

                clone.transform.GetChild(0).GetComponent<TMP_Text>().text = asset.name;

                clone.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(LoadCustomLevel(asset.id)));
                levels.Add(clone);



            }
        });

    }
    void Start()
    {
        LevelJson = null;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("logged in");
                //  SceneManager.LoadScene(3);
            }
            else
            {
                Debug.Log("FAILRE");
            }
        });
        LootLockerSDKManager.GetAssetListWithCount(10, (listResponse) =>
        {
            if (!listResponse.success)
            {
                Debug.LogError("Failed to get asset list");
                return;
            }

            foreach (var asset in listResponse.assets)
            {
                Debug.Log($"Asset name: {asset.name}");
                Debug.Log($"Asset id: {asset.id}");


                GameObject clone = Instantiate(button);

                clone.transform.parent = levelsList.transform;

                clone.transform.GetChild(0).GetComponent<TMP_Text>().text = asset.name;

                clone.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(LoadCustomLevel(asset.id)));
                levels.Add(clone);



            }
        });


    }
    IEnumerator DownloadFile(string url, string fileName, System.Action<string> onComplete)
    {
        Debug.Log("1");
        using (var req = UnityWebRequest.Get(url))
        {
            Debug.Log("2");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
                onComplete?.Invoke(null);
                yield break;
            }

            Debug.Log("3");
            byte[] data = req.downloadHandler.data;

            Debug.Log($"Downloaded {fileName}, {data.Length} bytes");

            string text = Encoding.UTF8.GetString(data);
            Debug.Log(text);
            Debug.Log("4");

            onComplete?.Invoke(text);
        }
    }


    public void NextPage()
    {
        foreach (var button in levels)
        {
            Destroy(button.gameObject);
        }
        GetLevels();
    }



    public IEnumerator LoadCustomLevel(int assetID)
    {
        bool isDone = false;
        string downloadedJson = null;
        Debug.Log($"Fetched asset:");
        LootLockerSDKManager.GetAssetById(assetID, (assetResponse) =>
        {
            if (!assetResponse.success)
            {
                Debug.LogError($"Failed to get asset {assetID}");
                isDone = true; // signal completion
                return;
            }

            Debug.Log($"Fetched asset: {assetResponse.asset.name}");

            // Assume you only download the first file for simplicity
            var file = assetResponse.asset.files[0];
            Debug.Log($"Downloading file: {assetResponse.asset.name}");
            Debug.Log($"URL: {file.url}");

            // Start the DownloadFile coroutine and get the result
            StartCoroutine(DownloadFile(file.url, assetResponse.asset.name, (json) =>
            {
                downloadedJson = json;
                isDone = true; // signal completion
            }));
        });

        // Wait until download finishes
        yield return new WaitUntil(() => isDone);

        if (!string.IsNullOrEmpty(downloadedJson))
        {
            LevelJson = downloadedJson;

            // Now you can safely load your level
            //LevelSelection.SelectedLevel = levelName;
            SceneManager.LoadScene("Level");
            Debug.Log("Download complete, ready to load level");
        }
    }


}
