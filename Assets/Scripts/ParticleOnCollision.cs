using UnityEngine;

public class ParticleOnCollision : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hitSound;
    public ParticleSystem hitParticle; // Assign your particle prefab here

    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        // Play the hit sound
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(hitSound);

        // Spawn particle at collision point
        if (hitParticle != null && collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0]; // First contact point
            ParticleSystem spawnedParticle = Instantiate(hitParticle, contact.point, Quaternion.identity);
            
            // Optional: Align particle to surface normal
            spawnedParticle.transform.rotation = Quaternion.LookRotation(contact.normal);
            
            // Destroy particle after its lifetime
            Destroy(spawnedParticle.gameObject, spawnedParticle.main.duration + spawnedParticle.main.startLifetime.constantMax);
        }
    }
}
