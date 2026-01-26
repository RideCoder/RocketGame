using UnityEngine;

public class ParticleOnCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        Debug.Log("what");
        Debug.Log(collision.contactCount);
    }
}
