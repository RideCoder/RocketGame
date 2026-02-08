using TMPro;
using UnityEngine;

public class DisplayGizmo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text text;
    public TMP_Text text2;
    public GizmoHandler handler;
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

        if (handler.currentHoveredGizmo != null)
        {
            text2.text = handler.currentHoveredGizmo.ToString();
        }
        else
        {
            text2.text = "null"; 
        }
    }
}
