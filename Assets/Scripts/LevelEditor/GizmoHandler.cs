using UnityEngine;
using UnityEngine.InputSystem;

public class GizmoHandler : MonoBehaviour
{
    public static GameObject gizmoSelected;

    public int mode = 0;

    public void ChangeMode(int m)
    {
        mode = m;
    }


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
            if (mode == 0)
            {
                if (gizmoSelected.name == "GizmoX")
                    LevelEditorController.targetObject.transform.position += new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime, 0, 0);
                if (gizmoSelected.name == "GizmoZ")
                    LevelEditorController.targetObject.transform.position += new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime);
                if (gizmoSelected.name == "GizmoY")
                    LevelEditorController.targetObject.transform.position += new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime, 0);
            }
            if (mode == 1)
            {
                if (gizmoSelected.name == "GizmoX")
                    LevelEditorController.targetObject.transform.localScale += new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime, 0, 0);
                if (gizmoSelected.name == "GizmoZ")
                    LevelEditorController.targetObject.transform.localScale += new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime);
                if (gizmoSelected.name == "GizmoY")
                    LevelEditorController.targetObject.transform.localScale += new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime, 0);
            }

        }
        else
        {
            GizmoUnselected();
        }
    }
}
