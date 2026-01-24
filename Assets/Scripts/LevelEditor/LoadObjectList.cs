using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadObjectList : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<GameObject> levelObjects = new List<GameObject>();
    public GameObject button;
    public static event Action<GameObject> OnObjectSelected;
    void Start()
    {

        levelObjects.AddRange(Resources.LoadAll<GameObject>("LevelObjects"));
        foreach (GameObject obj in levelObjects)
        {
            GameObject buttonClone = Instantiate(button);
            buttonClone.transform.SetParent(transform, false); // critical

            RectTransform rt = buttonClone.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;

            buttonClone.transform.GetChild(0).GetComponent<TMP_Text>().text = obj.name;

            buttonClone.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnObjectSelected?.Invoke(obj);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
