using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadObjectList : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private List<GameObject> levelObjects = new List<GameObject>();
    private List<KeyValuePair<string, GameObject>> objectButtons = new List<KeyValuePair<string, GameObject>>();
    public GameObject button;
    public string[] categories;
    public TMP_Text categoryText;
    public int categoryIndex = 0;
    public void IncreaseCategory()
    {
        categoryIndex++;
        if (categoryIndex >= categories.Length)
        {
            categoryIndex = 0;
        }
        categoryText.text = categories[categoryIndex];
        LoadCategory();
    }
    public void DecreaseCategory()
    {
        categoryIndex--;
        if (categoryIndex < 0)
        {
            categoryIndex = categories.Length-1;
        }
        categoryText.text = categories[categoryIndex];
        LoadCategory();
    }
    public static event Action<GameObject> OnObjectSelected;
    void Start()
    {
        levelObjects.AddRange(Resources.LoadAll<GameObject>("LevelObjects"));
        categoryText.text = categories[categoryIndex];
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
            
            objectButtons.Add(new KeyValuePair<string,GameObject>(obj.GetComponent<LevelObject>().categoryType, buttonClone));
        }

        LoadCategory();
    }

    public void LoadCategory()
    {
        foreach (KeyValuePair<string, GameObject> pair in objectButtons)
        {
            pair.Value.SetActive(false);
        }

        foreach (KeyValuePair<string, GameObject> pair in objectButtons)
        {
            if (pair.Key == categories[categoryIndex])
            {
                pair.Value.SetActive(true);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
