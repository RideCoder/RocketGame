using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolidObjectProperties : MonoBehaviour
{


    // ---------------- Color (HSV) ----------------
    public Slider hueSlider;
    public Slider saturationSlider;
    public Slider valueSlider;

    public void OnEnable()
    {
        Color color = LevelEditorController.targetObjects[0].GetComponent<SolidObject>().meshRenderer.materials[0].color;
        bool differentColor = false;
        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (!go.TryGetComponent(out SolidObject levelObject))
                continue;
            
            Color current = levelObject.meshRenderer.materials[0].color;
            if (color != current) 
                {
                differentColor = true;
                Debug.Log("Different color");
                } 
            
            


        }
        if (!differentColor)
        {
            Color.RGBToHSV(color, out float ch, out float cs, out float cv);

            hueSlider.value = ch;
            saturationSlider.value = cs;
            valueSlider.value = cv;
        }
        

    }
    public void ChangeHue(float h)
    {
        
        ApplyColorHSV(h, null, null);
    }

    public void ChangeSaturation(float s)
    {
        ApplyColorHSV(null, s, null);
    }

    public void ChangeValue(float v)
    {
        ApplyColorHSV(null, null, v);
    }

    private void ApplyColorHSV(float? h, float? s, float? v)
    {
        
        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            if (go == null)
                continue;

            if (!go.TryGetComponent(out SolidObject levelObject))
                continue;

            Color current = levelObject.meshRenderer.materials[0].color;

            Color.RGBToHSV(current, out float ch, out float cs, out float cv);

            float nh = h ?? ch;
            float ns = s ?? cs;
            float nv = v ?? cv;
            
            levelObject.meshRenderer.materials[0].color = Color.HSVToRGB(nh, ns, nv);
        }
    }

  
}
