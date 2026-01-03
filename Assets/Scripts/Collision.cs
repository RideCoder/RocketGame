using UnityEngine;

public class Collision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        Destroy(gameObject);
    }
}
