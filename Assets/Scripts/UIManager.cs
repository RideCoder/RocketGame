using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  //  public GameObject mainMenu;
  //  public GameObject optionsMenu;
   // public GameObject levelsMenu;

    public List<GameObject> menus;
    public GameObject escMenu;
    public static bool IsPaused => Time.timeScale == 0f;
    public void OpenMenu(GameObject menu)
    {
        foreach (var m in menus)
        {
            m.SetActive(false);
        }
        menu.SetActive(true);
    }


    public void Update()
    {
        if (escMenu != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame && !escMenu.activeSelf)
            {
                if (!GameManager.playerDead)
                {
                    Time.timeScale = 0;
                    escMenu.SetActive(true);
                }
              
            }
            else if(Keyboard.current.escapeKey.wasPressedThisFrame && escMenu.activeSelf)
            {
                Time.timeScale = 1;
                escMenu.SetActive(false);
                
            }
     
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        escMenu.SetActive(false);
    }

    public void GoToTitleScreen()
    {
        SceneManager.LoadScene("Main Menu");
    }


}
