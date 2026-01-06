using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PracticeMode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject practiceCheckpoint;
    public static Dictionary<int, GameObject> practiceCheckpoints = new Dictionary<int, GameObject>();
    public GameObject player;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIManager.IsPaused)
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                GameObject clone = Instantiate(practiceCheckpoint);
                clone.transform.position = player.transform.position;
                clone.GetComponent<Checkpoint>().velocity = player.GetComponent<Rigidbody>().linearVelocity;
                practiceCheckpoints.Add(practiceCheckpoints.Count + 1, clone);

            }

            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                practiceCheckpoints.TryGetValue(practiceCheckpoints.Count, out GameObject clone);
                practiceCheckpoints.Remove(practiceCheckpoints.Count);
                Destroy(clone);

            }
        }
    }
}
