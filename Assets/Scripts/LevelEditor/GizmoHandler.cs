using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;

public class GizmoHandler : MonoBehaviour
{
    public static GameObject gizmoSelected;
    public static GameObject plane1;
    public static GameObject plane2;
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
        plane1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plane1.layer = LayerMask.NameToLayer("Plane");
     
        plane1.SetActive(false);

        plane2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plane2.layer = LayerMask.NameToLayer("Plane");
        plane2.SetActive(false);

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
            Vector3 targetPos = LevelEditorController.targetObjects[0].transform.position;

            plane1.SetActive(true);
            plane2.SetActive(true);

            if (gizmoSelected.name == "GizmoX")
            {
                // XY plane and XZ plane for X-axis movement
                plane1.transform.localScale = new Vector3(100000f, .1f, 100000f);
                plane1.transform.position = targetPos;
                plane1.transform.rotation = Quaternion.identity;

                plane2.transform.localScale = new Vector3(100000f, 100000f, .1f);
                plane2.transform.position = targetPos;
                plane2.transform.rotation = Quaternion.identity;
            }
            else if (gizmoSelected.name == "GizmoZ")
            {
                // XZ plane and YZ plane for Z-axis movement
                plane1.transform.localScale = new Vector3(100000f, .1f, 100000f);
                plane1.transform.position = targetPos;
                plane1.transform.rotation = Quaternion.identity;

                plane2.transform.localScale = new Vector3(.1f, 100000f, 100000f);
                plane2.transform.position = targetPos;
                plane2.transform.rotation = Quaternion.identity;
            }
            else if (gizmoSelected.name == "GizmoY")
            {
                // XY plane and YZ plane for Y-axis movement
                plane1.transform.localScale = new Vector3(.1f, 100000f, 100000f);
                plane1.transform.position = targetPos;
                plane1.transform.rotation = Quaternion.identity;

                plane2.transform.localScale = new Vector3(100000f, 100000f, .1f);
                plane2.transform.position = targetPos;
                plane2.transform.rotation = Quaternion.identity;
            }
        }
    }

    public static void GizmoUnselected()
    {
        plane1.SetActive(false);
        plane2.SetActive(false);
        Debug.Log("Gizmo Unselected");
        gizmoSelected = null;
    }

    // Add these as class fields
    private Dictionary<GameObject, Vector3> objectOffsets = new Dictionary<GameObject, Vector3>();
    private bool isDragging = false;
    private Vector3 lastAvgPos;

    public void Update()
    {
        if (gizmoSelected == null) return;

        // Center gizmo on selected objects
        Vector3 avgPos = Vector3.zero;
        foreach (GameObject obj in LevelEditorController.targetObjects)
        {
            avgPos += obj.transform.position;
        }
        avgPos /= LevelEditorController.targetObjects.Count;
        gizmoSelected.transform.parent.position = avgPos;

        Ray ray = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Try to hit either plane
        RaycastHit planeHit;
        bool hitPlane = false;

        if (Physics.Raycast(ray, out planeHit, 1000f, LayerMask.GetMask("Plane")))
        {
            hitPlane = true;
        }

        if (hitPlane)
        {
            // On first frame of mouse press, store offsets
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("TEST");
                isDragging = true;
                objectOffsets.Clear();
                lastAvgPos = avgPos;

                // Store offset for each object from the gizmo center
                foreach (GameObject obj in LevelEditorController.targetObjects)
                {
                    objectOffsets[obj] = obj.transform.position - avgPos;
                }
            }

            if (Mouse.current.leftButton.isPressed)
            {
                // MOVE
                if (mode == 0)
                {
                    Vector3 newGizmoPos = avgPos; // Start with current position

                    // Calculate new gizmo position based on selected axis
                    if (gizmoSelected.name == "GizmoX")
                        newGizmoPos = new Vector3(planeHit.point.x, avgPos.y, avgPos.z);
                    else if (gizmoSelected.name == "GizmoZ")
                        newGizmoPos = new Vector3(avgPos.x, avgPos.y, planeHit.point.z);
                    else if (gizmoSelected.name == "GizmoY")
                        newGizmoPos = new Vector3(avgPos.x, planeHit.point.y, avgPos.z);

                    // Move all objects maintaining their relative positions
                    foreach (GameObject obj in LevelEditorController.targetObjects)
                    {
                        obj.transform.position = newGizmoPos + objectOffsets[obj];
                    }
                }
                // SCALE
                else if (mode == 1)
                {
                    foreach (GameObject obj in LevelEditorController.targetObjects)
                    {
                        if (gizmoSelected.name == "GizmoX")
                            obj.transform.localScale += new Vector3(Mouse.current.delta.ReadValue().x * Time.deltaTime, 0, 0);
                        else if (gizmoSelected.name == "GizmoZ")
                            obj.transform.localScale += new Vector3(0, 0, Mouse.current.delta.ReadValue().x * Time.deltaTime);
                        else if (gizmoSelected.name == "GizmoY")
                            obj.transform.localScale += new Vector3(0, Mouse.current.delta.ReadValue().y * Time.deltaTime, 0);
                    }
                }
                // ROTATE
                else if (mode == 2)
                {
                    foreach (GameObject obj in LevelEditorController.targetObjects)
                    {
                        if (gizmoSelected.name == "GizmoX")
                            obj.transform.RotateAround(avgPos, Vector3.right, Mouse.current.delta.ReadValue().x * Time.deltaTime * 90);
                        else if (gizmoSelected.name == "GizmoZ")
                            obj.transform.RotateAround(avgPos, Vector3.forward, Mouse.current.delta.ReadValue().x * Time.deltaTime * 90);
                        else if (gizmoSelected.name == "GizmoY")
                            obj.transform.RotateAround(avgPos, Vector3.up, Mouse.current.delta.ReadValue().y * Time.deltaTime * 90);
                    }
                }
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
            }
        }
        else
        {
            if (!Mouse.current.leftButton.isPressed)
            {
                GizmoUnselected();
            }
        }
    }
}