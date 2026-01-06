using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    
    public void ChooseLevel(int level)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level "+level.ToString());
    }
}
