using UnityEngine;
using UnityEngine.InputSystem;

public class GizmoHandler : MonoBehaviour
{
    public static GameObject gizmoSelected;
    public static void GizmoSelected(GameObject gizmo)
    {
        Debug.Log("Gizmo Selected");
        gizmoSelected = gizmo;
    }

    public static void GizmoUnselected()
    {
        Debug.Log("Gizmo Unselected");
        gizmoSelected = null;
    }
    public void Update()
    {
        if (gizmoSelected == null)
        {
            
            return;
        }
        gizmoSelected.transform.parent.transform.position = LevelEditorController.targetObject.transform.position;

        if (Mouse.current.leftButton.isPressed)
        {
            
            if (gizmoSelected.name == "GizmoX")
                LevelEditorController.targetObject.transform.position += new Vector3(Mouse.current.delta.ReadValue().x*Time.deltaTime, 0, 0);
            if (gizmoSelected.name == "GizmoZ")
                LevelEditorController.targetObject.transform.position += new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime);
            if (gizmoSelected.name == "GizmoY")
                LevelEditorController.targetObject.transform.position += new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime, 0);
        }
        else
        {
            GizmoUnselected();
        }
    }
}
