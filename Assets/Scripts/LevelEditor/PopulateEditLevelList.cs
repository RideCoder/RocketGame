using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopulateEditLevelList : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject levelsList;
    public GameObject button;
    public GameObject newLevelButton;


    public void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Levels");

        var info = new DirectoryInfo(path);
        newLevelButton.GetComponent<Button>().onClick.AddListener(() => LoadCustomLevel(""));
        if (info.GetFiles().Length == 0)
        {
            Debug.Log("No levels found");
            return;
        }
        foreach (var file in info.GetFiles())
        {

            if (file.Extension == ".json")
            {
                GameObject clone = Instantiate(button);
                clone.transform.parent = levelsList.transform;

                clone.transform.GetChild(0).GetComponent<TMP_Text>().text = Path.GetFileNameWithoutExtension(file.Name);

                clone.GetComponent<Button>().onClick.AddListener(() => LoadCustomLevel(file.Name));
            }

        }





    }

    public void LoadCustomLevel(string levelName)
    {
        LevelSelection.SelectedLevel = levelName;
        SceneManager.LoadScene("Level Editor");
    }

}
