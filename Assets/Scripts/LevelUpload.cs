using LootLocker.Requests;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class LevelUpload : MonoBehaviour
{
    public TMP_InputField levelNameInputField;
    public LevelSerializer levelSerializer;

    private string levelName;

    public void CreateLevel()
    {
        levelName = levelNameInputField.text;

        LootLockerSDKManager.CreatingAnAssetCandidate(levelName, (response) =>
        {
            if (response.success)
            {
                UploadLevelData(response.asset_candidate_id);
            }
            else
            {
                Debug.LogError("Failed to create asset candidate: " + response.text);
            }
        });
    }

    private void UploadLevelData(int levelID)
    {
        string json = levelSerializer.SerializeToJson();

        // Save temporary file to persistentDataPath
        string dir = Path.Combine(Application.persistentDataPath, "TempLevels");
        Directory.CreateDirectory(dir);

        string path = Path.Combine(dir, levelSerializer.levelName + ".json");
        File.WriteAllText(path, json);

        // Upload using the path
        LootLockerSDKManager.AddingFilesToAssetCandidates(
            levelID,
            path,
            levelSerializer.levelName + ".json",
            LootLocker.LootLockerEnums.FilePurpose.file,
            (textResponse) =>
            {
                if (textResponse.success)
                {
                    Debug.Log("Upload successful!");

                    // Approve the asset candidate
                    LootLockerSDKManager.UpdatingAnAssetCandidate(levelID, true, (updateResponse) =>
                    {
                        if (updateResponse.success)
                            Debug.Log("Asset candidate approved!");
                        else
                            Debug.LogError("Failed to approve asset candidate: " + updateResponse.text);
                    });

                    // Delete temporary file
                    try
                    {
                        File.Delete(path);
                    }
                    catch { }
                }
                else
                {
                    Debug.LogError("Upload failed: " + textResponse.text);
                }
            });
    }

}
