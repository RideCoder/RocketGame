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
        Vector3 avgPos = new Vector3(0, 0, 0);
        foreach (GameObject obj in LevelEditorController.targetObjects)
        {
            avgPos += obj.transform.position;
        }
        avgPos = avgPos / LevelEditorController.targetObjects.Count;
        gizmoSelected.transform.parent.transform.position = avgPos;

        if (Mouse.current.leftButton.isPressed)
        {
            foreach (GameObject obj in LevelEditorController.targetObjects)
            {


                if (mode == 0)
                {
                    if (gizmoSelected.name == "GizmoX")
                        obj.transform.position += new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime, 0, 0);
                    if (gizmoSelected.name == "GizmoZ")
                        obj.transform.position += new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime);
                    if (gizmoSelected.name == "GizmoY")
                        obj.transform.position += new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime, 0);
                }
                if (mode == 1)
                {
                    if (gizmoSelected.name == "GizmoX")
                        obj.transform.localScale += new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime, 0, 0);
                    if (gizmoSelected.name == "GizmoZ")
                        obj.transform.localScale += new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime);
                    if (gizmoSelected.name == "GizmoY")
                        obj.transform.localScale += new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime, 0);
                }
                if (mode == 2)
                {
                    if (gizmoSelected.name == "GizmoX")
                        obj.transform.Rotate(new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime * 360, 0, 0));
                    if (gizmoSelected.name == "GizmoZ")
                        obj.transform.Rotate(new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime * 360));
                    if (gizmoSelected.name == "GizmoY")
                        obj.transform.Rotate(new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime * 360, 0));
                }
            }
        }
        else
        {
            GizmoUnselected();
        }
    }
}
