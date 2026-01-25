using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class GizmoHandler : MonoBehaviour
{
    public static GameObject gizmoSelected;
    public static GameObject plane1;
    public static GameObject plane2;
    public TMP_Text snapText;
    public Material material;
    public static int mode = 0;
    public Camera editorCamera;
    public TMP_Text[] editorButtons;

    // Snapping settings
    public static bool snapEnabled = true;
    public float positionSnapSize = 1f;
    public float rotationSnapSize = 15f;
    public float scaleSnapSize = 1f;
    static float rotationAmount = 0f;

    // Add these as class fields
    private Dictionary<GameObject, Vector3> objectOffsets = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> initialScales = new Dictionary<GameObject, Vector3>();
    private bool isDragging = false;
    private Vector3 lastAvgPos;
    private Vector3 dragStartPoint;
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        // For mouse
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        // For Input System touch / pointer IDs
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed &&
                    EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue()))
                {
                    return true;
                }
            }
        }

        return false;
    }
    public void ChangeMode(int m)
    {
        mode = m;
        foreach (TMP_Text text in editorButtons)
        {
            text.color = Color.white;
        }
        editorButtons[m].color = new UnityEngine.Color(60f / 255f, 93f / 255f, 255, 255);
    }

    public void ToggleSnap()
    {
        snapEnabled = !snapEnabled;
        if (snapEnabled)
        {
            snapText.color = Color.green;
        }
        else
        {
            snapText.color = Color.red;
        }
    }
            public void Start()
            {
        snapEnabled = true;
                plane1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                plane1.layer = LayerMask.NameToLayer("Plane");
                MakeTransparent(plane1);
                plane1.SetActive(false);

                plane2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                plane2.layer = LayerMask.NameToLayer("Plane");
                MakeTransparent(plane2);
                plane2.SetActive(false);

                foreach (TMP_Text text in editorButtons)
                {
                    text.color = Color.white;
                }
                editorButtons[mode].color = new UnityEngine.Color(60f / 255f, 93f / 255f, 255, 255);
            }

    private void MakeTransparent(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
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
        // SCALE
        else if (mode == 1)
        {
            Vector3 targetPos = LevelEditorController.targetObjects[0].transform.position;

            plane1.SetActive(true);
            plane2.SetActive(true);

            if (gizmoSelected.name == "GizmoX")
            {
                // XY plane and XZ plane for X-axis scaling
                plane1.transform.localScale = new Vector3(100000f, .1f, 100000f);
                plane1.transform.position = targetPos;
                plane1.transform.rotation = Quaternion.identity;

                plane2.transform.localScale = new Vector3(100000f, 100000f, .1f);
                plane2.transform.position = targetPos;
                plane2.transform.rotation = Quaternion.identity;
            }
            else if (gizmoSelected.name == "GizmoZ")
            {
                // XZ plane and YZ plane for Z-axis scaling
                plane1.transform.localScale = new Vector3(100000f, .1f, 100000f);
                plane1.transform.position = targetPos;
                plane1.transform.rotation = Quaternion.identity;

                plane2.transform.localScale = new Vector3(.1f, 100000f, 100000f);
                plane2.transform.position = targetPos;
                plane2.transform.rotation = Quaternion.identity;
            }
            else if (gizmoSelected.name == "GizmoY")
            {
                // XY plane and YZ plane for Y-axis scaling
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
        rotationAmount = 0;
        gizmoSelected = null;
    }

    private float SnapValue(float value, float snapSize)
    {
        if (!snapEnabled) return value;
        return Mathf.Round(value / snapSize) * snapSize;
    }

    private Vector3 SnapVector3(Vector3 value, float snapSize, Vector3 origin)
    {
        float Snap(float v, float o)
        {
            return Mathf.Round((v - o) / snapSize) * snapSize + o;
        }
        return new Vector3(
        Snap(value.x, origin.x),
        Snap(value.y, origin.y),
            Snap(value.z, origin.z)
        );
    }
    private GameObject currentHoveredGizmo;
    private Vector3 originalScale;
    [SerializeField] private float hoverScaleMultiplier = 1.2f;
    public void HoverOverGizmo(GameObject gizmo)
    {
        // If we're already hovering this gizmo, do nothing
        if (currentHoveredGizmo == gizmo)
            return;

        // Restore previous gizmo scale
        if (currentHoveredGizmo != null)
        {
            currentHoveredGizmo.transform.localScale = originalScale;

        }

        // Assign new hovered gizmo
        currentHoveredGizmo = gizmo;
        originalScale = gizmo.transform.localScale;

        // Apply hover scale
        gizmo.transform.localScale = originalScale * hoverScaleMultiplier;
    }

    private void ClearHover()
    {
        if (currentHoveredGizmo != null)
        {
            currentHoveredGizmo.transform.localScale = originalScale;
            currentHoveredGizmo = null;
        }
    }

    private void Update()
    {
        // Ignore all gizmo interactions while pointer is over UI
        if (IsPointerOverUI())
        {
            ClearHover();

            if (Mouse.current.leftButton.wasReleasedThisFrame)
                isDragging = false;

            return;
        }
        if (!isDragging)
        {
            Ray gizmoRay = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(gizmoRay, out RaycastHit hit, 100f, LayerMask.GetMask("Gizmo")))
            {
                HoverOverGizmo(hit.collider.gameObject.transform.parent.gameObject);
            }
            else
            {
                ClearHover();
            }

        }


        if (gizmoSelected == null) return;

        // Center gizmo on selected objects
        Vector3 avgPos = Vector3.zero;
        foreach (GameObject obj in LevelEditorController.targetObjects)
        {
            avgPos += obj.transform.position;
        }
        if (LevelEditorController.targetObjects.Count != 0)
        {
            avgPos /= LevelEditorController.targetObjects.Count;
            gizmoSelected.transform.parent.position = avgPos;
        }

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
            if (Mouse.current.leftButton.wasPressedThisFrame ||
                (Mouse.current.leftButton.isPressed && !isDragging))
            {
                isDragging = true;
                objectOffsets.Clear();
                initialScales.Clear();
                lastAvgPos = avgPos;
                dragStartPoint = planeHit.point;

                // MOVE mode - Store position offset for each object from the gizmo center
                if (mode == 0)
                {
                    foreach (GameObject obj in LevelEditorController.targetObjects)
                    {
                        objectOffsets[obj] = obj.transform.position - avgPos;
                    }
                }
                // SCALE mode - Store initial scale for each object
                else if (mode == 1)
                {
                    foreach (GameObject obj in LevelEditorController.targetObjects)
                    {
                        initialScales[obj] = obj.transform.localScale;
                    }
                }
            }

            // Update plane positions to follow the objects while dragging
            if (isDragging && mode == 0)
            {
                if (gizmoSelected.name == "GizmoX")
                {
                    plane1.transform.position = avgPos;
                    plane2.transform.position = avgPos;
                }
                else if (gizmoSelected.name == "GizmoZ")
                {
                    plane1.transform.position = avgPos;
                    plane2.transform.position = avgPos;
                }
                else if (gizmoSelected.name == "GizmoY")
                {
                    plane1.transform.position = avgPos;
                    plane2.transform.position = avgPos;
                }
            }

            if (Mouse.current.leftButton.isPressed && isDragging)
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
                    // Apply snapping to the new position
                    if (snapEnabled)
                    {
                        newGizmoPos = SnapVector3(newGizmoPos, positionSnapSize, avgPos);
                    }
                   

                        // Move all objects maintaining their relative positions
                        foreach (GameObject obj in LevelEditorController.targetObjects)
                        {
                            if (objectOffsets.ContainsKey(obj))
                            {
                                obj.transform.position = newGizmoPos + objectOffsets[obj];
                            }
                        }
                }
                // SCALE
                else if (mode == 1)
                {
                    // Calculate scale factor based on plane hit distance from starting position
                    float scaleAmount = 0f;

                    if (gizmoSelected.name == "GizmoX")
                        scaleAmount = planeHit.point.x - dragStartPoint.x;
                    else if (gizmoSelected.name == "GizmoZ")
                        scaleAmount = planeHit.point.z - dragStartPoint.z;
                    else if (gizmoSelected.name == "GizmoY")
                        scaleAmount = planeHit.point.y - dragStartPoint.y;

                    foreach (GameObject obj in LevelEditorController.targetObjects)
                    {
                        if (initialScales.ContainsKey(obj))
                        {
                            Vector3 newScale = initialScales[obj]; // Use stored initial scale
                            Vector3 tempScale = newScale;
                            if (gizmoSelected.name == "GizmoX")
                                newScale.x = initialScales[obj].x + scaleAmount;
                            else if (gizmoSelected.name == "GizmoZ")
                                newScale.z = initialScales[obj].z + scaleAmount;
                            else if (gizmoSelected.name == "GizmoY")
                                newScale.y = initialScales[obj].y + scaleAmount;

                            // Prevent negative scaling
                            newScale.x = Mathf.Max(0.01f, newScale.x);
                            newScale.y = Mathf.Max(0.01f, newScale.y);
                            newScale.z = Mathf.Max(0.01f, newScale.z);
                            if (snapEnabled)
                            {
                                obj.transform.localScale = SnapVector3(newScale, scaleSnapSize, tempScale);
                            }
                            else
                            {
                                obj.transform.localScale = newScale;
                            }
                            
                        }
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

        if (Mouse.current.leftButton.isPressed)
        {
            // ROTATE
            if (mode == 2)
            {
                float rotationDelta = 0f;
                Vector3 rotationAxis = Vector3.zero;

                if (gizmoSelected.name == "GizmoX")
                {
                    rotationDelta = Mouse.current.delta.ReadValue().x * Time.deltaTime * 90;
                    rotationAxis = Vector3.right;
                }
                else if (gizmoSelected.name == "GizmoZ")
                {
                    rotationDelta = Mouse.current.delta.ReadValue().x * Time.deltaTime * 90;
                    rotationAxis = Vector3.forward;
                }
                else if (gizmoSelected.name == "GizmoY")
                {
                    rotationDelta = Mouse.current.delta.ReadValue().y * Time.deltaTime * 90;
                    rotationAxis = Vector3.up;
                }

                if (snapEnabled)
                {
                    rotationAmount += rotationDelta;

                    // Only rotate when we've accumulated enough rotation
                    if (Mathf.Abs(rotationAmount) >= rotationSnapSize)
                    {
                        // Calculate how many snap increments to apply
                        float snappedRotation = Mathf.Floor(Mathf.Abs(rotationAmount) / rotationSnapSize) * rotationSnapSize * Mathf.Sign(rotationAmount);

                        // Apply the rotation to all objects at once
                        foreach (GameObject obj in LevelEditorController.targetObjects)
                        {
                            obj.transform.RotateAround(avgPos, rotationAxis, snappedRotation);
                        }

                        // Subtract the applied rotation from the accumulator
                        rotationAmount -= snappedRotation;
                    }
                }
                else
                {
                    // When snapping is disabled, apply rotation directly to all objects
                    foreach (GameObject obj in LevelEditorController.targetObjects)
                    {
                        obj.transform.RotateAround(avgPos, rotationAxis, rotationDelta);
                    }
                }
            }
        }
    }
}