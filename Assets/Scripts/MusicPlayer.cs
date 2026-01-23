using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource; // Drag your AudioSource component here in the Inspector
    public float startTime = 95.0f; // The time in seconds to start

    void Start()
    {
        // Ensure "Play On Awake" is unchecked in the AudioSource component in the Inspector
        if (audioSource != null)
        {
            audioSource.time = startTime; // Set the starting point
            audioSource.Play(); // Play the audio
        }
    }
}
