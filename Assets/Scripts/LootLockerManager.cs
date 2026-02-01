using LootLocker.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LootLockerManager : MonoBehaviour
{
    public void Login()
    {
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
    }

}
