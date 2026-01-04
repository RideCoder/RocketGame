
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public GameObject rocket;

    public static event Action OnPlayerDeath;
    public static event Action OnPlayerTouchGoal;
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
       if (collision.gameObject.GetComponent<Goal>() == null)
        {
            OnPlayerDeath?.Invoke();
        }
        else
        {
            OnPlayerTouchGoal?.Invoke();
        }
        
    }
}