using UnityEngine;

public class Rotater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame




    public float xAxisSpeed;
    public float yAxisSpeed;
    public float zAxisSpeed;
    void Update()
    {
        
            transform.Rotate(xAxisSpeed*Time.deltaTime,yAxisSpeed * Time.deltaTime, zAxisSpeed * Time.deltaTime); 
        
    }
}
