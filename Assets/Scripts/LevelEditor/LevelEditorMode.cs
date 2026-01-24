using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorMode : MonoBehaviour
{

    public GameObject[] ActiveOnPlay;
    public GameObject[] ActiveOnEdit;
    public Button modeSwitcherButton;
    public Camera cam;
    public Vector3 editorPosition;
    public Quaternion editorRotation;

    
    private enum Mode
    {
        Play,
        Edit
    }

    private Mode mode;
    public void Start()
    {
        
        mode = Mode.Edit;
        EditMode();

    }
    public void EditMode()
    {
        foreach (GameObject go in ActiveOnPlay)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in ActiveOnEdit)
        {
            go.SetActive(true);
        }
    }

    public void PlayMode()
    {
        GameObject player = new GameObject();
        foreach (GameObject go in ActiveOnPlay)
        {
            
            go.SetActive(true);

            if (go.TryGetComponent<Player>(out Player plr))
            {
                player = plr.gameObject;
                
            }
        }
        foreach (GameObject go in ActiveOnEdit)
        {
            go.SetActive(false);
        }

        GameManager.Instance.SetSpawnPoint();
        player.transform.position = GameManager.Instance.spawnPoint.gameObject.transform.position;

    }

    public void SwitchMode()
    {
        if (mode == Mode.Play)
        {
            cam.transform.position = editorPosition;
            cam.transform.rotation = editorRotation;
            GameManager.Instance.player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            mode = Mode.Edit;
            EditMode();
            modeSwitcherButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Test Mode";
            
        }

        else if (mode == Mode.Edit)
        {
            editorPosition = cam.transform.position;
            editorRotation = cam.transform.rotation;
            mode = Mode.Play;
            PlayMode();
            LevelEditorController.Instance.ClearSelection();

            modeSwitcherButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Edit Mode";
          //  GameManager.Instance.elapsedTime = 0f;     // Reset timer on death
        }

    }
    
}
