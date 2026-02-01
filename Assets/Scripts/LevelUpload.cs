using LootLocker.Requests;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpload : MonoBehaviour
{
    public TMP_InputField levelNameInputField;
    private string levelName;
    public LevelSerializer levelSerializer;
  
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
                Debug.Log(" REALY BAD");
            }
        });
    }

    public void UploadLevelData(int levelID)
    {
        // Save the level locally
        levelSerializer.Save();

        // Construct the file path
        string path = Path.Combine(Application.streamingAssetsPath, "Levels", levelSerializer.levelName + ".json");

        // Upload the file to LootLocker
        LootLockerSDKManager.AddingFilesToAssetCandidates(
            levelID,
            path,
            levelSerializer.levelName + ".json",
            LootLocker.LootLockerEnums.FilePurpose.file,
            (textresponse) =>
            {
                if (textresponse.success)
                {
                    Debug.Log("Upload successful!");

                    // Update the asset candidate as approved
                    LootLockerSDKManager.UpdatingAnAssetCandidate(levelID, true, (updatedResponse) =>
                    {
                        if (updatedResponse.success)
                            Debug.Log("Asset candidate updated successfully!");
                        else
                            Debug.LogError("Failed to update asset candidate: " + updatedResponse.text);
                    });

                    // Delete the local file
                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                            Debug.Log("Local file deleted successfully.");
                        }
                        catch (IOException e)
                        {
                            Debug.LogError("Failed to delete local file: " + e.Message);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("File not found for deletion: " + path);
                    }
                }
                else
                {
                    Debug.LogError("Upload failed: " + textresponse.text);
                }
            });
    }

}
