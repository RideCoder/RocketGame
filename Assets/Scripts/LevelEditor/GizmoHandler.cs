using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;

public class GizmoHandler : MonoBehaviour
{
    public static GameObject gizmoSelected;
    public static GameObject plane;
    public static int mode = 0;
    public Camera editorCamera;
    public TMP_Text[] editorButtons;
    public void ChangeMode(int m)
    {
        mode = m;
        foreach (TMP_Text text in editorButtons)
        {
            text.color = Color.white;
        }

        editorButtons[m].color = new UnityEngine.Color(60f / 255f, 93f / 255f, 255, 255);
    }
    public void Start()
    {

        plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plane.layer = LayerMask.NameToLayer("Plane");
        foreach (TMP_Text text in editorButtons)
        {
            text.color = Color.white;
        }

        editorButtons[mode].color = new UnityEngine.Color(60f / 255f, 93f / 255f, 255, 255);
    }
    public static void GizmoSelected(GameObject gizmo)
    {
        Debug.Log("Gizmo Selected");
        gizmoSelected = gizmo;
        // MOVE
        if (mode == 0)
        {
            plane.SetActive(true);

            if (gizmoSelected.name == "GizmoX")
            {
                plane.transform.localScale = new Vector3(100f, .1f, 100f);
                plane.transform.position = LevelEditorController.targetObjects[0].transform.position;
            }
            if (gizmoSelected.name == "GizmoZ")
            {
                plane.transform.localScale = new Vector3(100f, .1f, 100f);
                plane.transform.position = LevelEditorController.targetObjects[0].transform.position;
            }

            if (gizmoSelected.name == "GizmoY")
            {
                plane.transform.localScale = new Vector3(.1f, 100f, 100f);
                plane.transform.position = LevelEditorController.targetObjects[0].transform.position;
            }
        }

      

        
       
    }

    public static void GizmoUnselected()
    {
        plane.SetActive(false);
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
        Ray ray = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit planeHit, 1000f, LayerMask.GetMask("Plane")))
        {
       
                if (Mouse.current.leftButton.isPressed)
                {
                    foreach (GameObject obj in LevelEditorController.targetObjects)
                    {
                        // MOVE
                        if (mode == 0)
                        {
                        if (gizmoSelected.name == "GizmoX")
                            
                            obj.transform.position = new Vector3(planeHit.point.x, obj.transform.position.y, obj.transform.position.z);

                            //  if (gizmoSelected.name == "GizmoZ")


                            //  if (gizmoSelected.name == "GizmoY")

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
                                obj.transform.RotateAround(avgPos, Vector3.right,
                                    Mouse.current.delta.ReadValue().x * Time.deltaTime * 90);

                            if (gizmoSelected.name == "GizmoZ")
                                obj.transform.RotateAround(avgPos, Vector3.forward,
                                    Mouse.current.delta.ReadValue().x * Time.deltaTime * 90);

                            if (gizmoSelected.name == "GizmoY")
                                obj.transform.RotateAround(avgPos, Vector3.up,
                                    Mouse.current.delta.ReadValue().y * Time.deltaTime * 90);
                        }
                    
                }
            }
        }
        else
        {
            
            GizmoUnselected();
        }
    }
}
