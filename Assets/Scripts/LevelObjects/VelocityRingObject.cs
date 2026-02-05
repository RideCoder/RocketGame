using System.Collections;
using UnityEngine;

public class VelocityRingObject : LevelObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool hit = false;
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<PlayerController>(out PlayerController plr) && !hit) 
        {
            hit = true;
            Debug.Log("HIT");
            Vector3 pushDir = transform.up.normalized;
            Debug.Log(pushDir);
            plr.rb.linearVelocity += pushDir * 30f;
            StartCoroutine(Reset());
        }
    }


    IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);
        hit = false;
        
    }
}
