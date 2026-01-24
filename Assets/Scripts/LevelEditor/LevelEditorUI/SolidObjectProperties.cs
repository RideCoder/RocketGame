using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class SolidObjectProperties : MonoBehaviour
{
   

    // ---------------- Color (HSV) ----------------

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
