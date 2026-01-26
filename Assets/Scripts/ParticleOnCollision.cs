using UnityEngine;

public class ParticleOnCollision : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hitSound;

    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(hitSound);
    }
}
