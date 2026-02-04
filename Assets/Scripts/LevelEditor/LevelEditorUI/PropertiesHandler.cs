using System;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesHandler : MonoBehaviour
{
    public GameObject propertiesButton;
   

    public List<LevelObject> selectedObjects = new List<LevelObject>();
    public GameObject lightProperties;
    public GameObject solidObjectProperties;
    public GameObject speedChangeProperties;
    public GameObject bloomObjectProperties;
    public GameObject colorAdjustmentObjectProperties;
    

    public void OpenProperties()
    {
        selectedObjects.Clear();

        Type selectedType = null;

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (!go.TryGetComponent(out LevelObject levelObject))
                continue;

            Type currentType = levelObject.GetType();

            // First valid object sets the expected type
            if (selectedType == null)
            {
                selectedType = currentType;
            }
            // Any mismatch → abort
            else if (currentType != selectedType)
            {
                Debug.Log("Cannot open properties: mixed LevelObject types selected.");
                return;
            }

            selectedObjects.Add(levelObject);
        }

        if (selectedObjects.Count == 0)
            return;

        // All selected objects share the same derived type
        ShowPropertiesForType(selectedType);
    }

    private void ShowPropertiesForType(Type type)
    {
        if (type == typeof(LightObject))
        {
            lightProperties.SetActive(true);
        }
        if (type == typeof(SolidObject))
        {
            solidObjectProperties.SetActive(true);
        }
        if (type == typeof(SpeedChangeObject))
        {
            speedChangeProperties.SetActive(true);
        }
        if (type == typeof(BloomObject))
        {
            bloomObjectProperties.SetActive(true);
        }
        if (type == typeof(ColorAdjustmentObject))
        {
            colorAdjustmentObjectProperties.SetActive(true);
        }
        Debug.Log($"Opening properties for type: {type.Name}");

        // Example:
        // if (type == typeof(LightObject)) { ShowLightProperties(); }
        // else if (type == typeof(EnemyObject)) { ShowEnemyProperties(); }
    }
}
