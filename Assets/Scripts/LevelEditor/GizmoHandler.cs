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
            return;

        // Center gizmo on selected objects
        Vector3 avgPos = Vector3.zero;
        foreach (GameObject obj in LevelEditorController.targetObjects)
        {
            avgPos += obj.transform.position;
        }

        avgPos /= LevelEditorController.targetObjects.Count;
        gizmoSelected.transform.parent.position = avgPos;

        if (Mouse.current.leftButton.isPressed)
        {
            foreach (GameObject obj in LevelEditorController.targetObjects)
            {
                // MOVE
                if (mode == 0)
                {
                    if (gizmoSelected.name == "GizmoX")
                        obj.transform.position +=
                            new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime, 0, 0);

                    if (gizmoSelected.name == "GizmoZ")
                        obj.transform.position +=
                            new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime);

                    if (gizmoSelected.name == "GizmoY")
                        obj.transform.position +=
                            new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime, 0);
                }

                // SCALE
                if (mode == 1)
                {
                    if (gizmoSelected.name == "GizmoX")
                        obj.transform.localScale +=
                            new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime, 0, 0);

                    if (gizmoSelected.name == "GizmoZ")
                        obj.transform.localScale +=
                            new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime);

                    if (gizmoSelected.name == "GizmoY")
                        obj.transform.localScale +=
                            new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime, 0);
                }

                // ROTATE
                if (mode == 2)
                {
                    if (gizmoSelected.name == "GizmoX")
                        obj.transform.Rotate(
                            new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime * 90, 0, 0));

                    if (gizmoSelected.name == "GizmoZ")
                        obj.transform.Rotate(
                            new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime * 90));

                    if (gizmoSelected.name == "GizmoY")
                        obj.transform.Rotate(
                            new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime * 90, 0));
                }
            }
        }
        else
        {
            GizmoUnselected();
        }
    }
}
