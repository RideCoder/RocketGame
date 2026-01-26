using UnityEngine;

public class ForceArrow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.transform.parent.TryGetComponent<PlayerController>(out PlayerController plr))
        {

            
        }

    }
}
