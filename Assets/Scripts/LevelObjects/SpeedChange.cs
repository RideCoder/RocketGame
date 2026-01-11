using UnityEngine;

public class SpeedChange : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speedChange;
    public float thrustSpeed;
    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.transform.parent.TryGetComponent<PlayerController>(out PlayerController plr))
        {
            Debug.Log("TEST");
            plr.forwardSpeed = speedChange;
            plr.thrustForce = thrustSpeed;
        }

    }
}
