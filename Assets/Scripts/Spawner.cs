using UnityEngine;
using UnityEngine.Experimental.AI;

public class Spawner : MonoBehaviour
{
    public GameObject spawner;
    public GameObject orb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject clone = Instantiate(orb);
            clone.transform.position = new Vector3(Random.Range(-gameObject.transform.localScale.x/2, gameObject.transform.localScale.x/2), Random.Range(-gameObject.transform.localScale.y / 2, gameObject.transform.localScale.y / 2), Random.Range(-gameObject.transform.localScale.z / 2, gameObject.transform.localScale.z / 2)) + transform.position;

        }
    }   

   
}
