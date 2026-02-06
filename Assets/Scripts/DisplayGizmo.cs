using TMPro;
using UnityEngine;

public class DisplayGizmo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text text;

    public void Update()
    {
        if (GizmoHandler.gizmoSelected != null)
        {
            text.text = GizmoHandler.gizmoSelected.ToString();
        }
        else
        {
            text.text = "null";
        }
    }
}
