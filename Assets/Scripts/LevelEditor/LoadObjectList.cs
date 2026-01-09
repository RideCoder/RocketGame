using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadObjectList : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject[] objects;
    public GameObject button;
    public static event Action<GameObject> OnObjectSelected;
    void Start()
    {
        foreach (GameObject obj in objects)
        {
            GameObject buttonClone = Instantiate(button);
            buttonClone.transform.parent = transform;
            buttonClone.transform.GetChild(0).GetComponent<TMP_Text>().text = obj.name;
            buttonClone.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnObjectSelected.Invoke(obj);
               
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
