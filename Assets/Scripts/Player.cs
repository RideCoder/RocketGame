
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public GameObject rocket;

    public static event Action OnPlayerDeath;
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
       
        OnPlayerDeath?.Invoke();
    }
}