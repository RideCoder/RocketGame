using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class LevelEditorController : MonoBehaviour
{
    // --------------------------------------------------
    // References
    // --------------------------------------------------
    [Header("References")]
    public Camera editorCamera;
    public Transform levelParent;
    public static GameObject selectedObject;
    public GameObject startObject;

    // --------------------------------------------------
    // Placement
    // --------------------------------------------------
    [Header("Placement Settings")]
    public float gridSize = 1f;
    public LayerMask placementMask;

    // --------------------------------------------------
    // Selection / Raycasting
    // --------------------------------------------------
    [Header("Raycast Masks")]
    public LayerMask gizmoMask;
    public LayerMask objectMask;
    public bool dragging;
    public Vector2 dragStart;
    public Vector2 dragEnd;
    public Rect selectionBox;
    // --------------------------------------------------
    // Ghost
    // --------------------------------------------------
    [Header("Ghost Settings")]
    public Material ghostMaterial;

    GameObject ghost;
    MeshRenderer ghostRenderer;

    // --------------------------------------------------
    // Gizmo
    // --------------------------------------------------
    public GameObject positionGizmo;
    public static List<GameObject> targetObjects = new List<GameObject>();

    // --------------------------------------------------
    // Mode
    // --------------------------------------------------
    public enum Mode
    {
        Select,
        Place
    }

    public Mode currentMode;
    public static LevelEditorController Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }



    public void SetSelectMode()
    {
        ghost.SetActive(false);
        currentMode = Mode.Select;
    }

    public void SetPlaceMode()
    {
        GizmoHandler.GizmoUnselected();
        positionGizmo.SetActive(false);

        foreach (GameObject obj in targetObjects)
            if (obj != null)
            {
                obj.layer = 0;
            }

        targetObjects.Clear();
        currentMode = Mode.Place;
    }

    // --------------------------------------------------
    // Unity Lifecycle
    // --------------------------------------------------
    void Start()
    {
        LoadObjectList.OnObjectSelected += UpdateSelected;
        selectedObject = startObject;
        CreateGhost();
        dragging = false;
    }
    void OnGUI()
    {
        if (!dragging)
            return;

        Vector2 currentMouse = Mouse.current.position.ReadValue();

        float xMin = Mathf.Min(dragStart.x, currentMouse.x);
        float yMin = Screen.height - Mathf.Max(dragStart.y, currentMouse.y);
        float width = Mathf.Abs(dragStart.x - currentMouse.x);
        float height = Mathf.Abs(dragStart.y - currentMouse.y);

        Rect rect = new Rect(xMin, yMin, width, height);

        // Fill
   

        // Border
        DrawRectBorder(rect, 2, UnityEngine.Color.cyan);
    }
    void DrawRectBorder(Rect rect, float thickness, UnityEngine.Color color)
    {
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color); // Top
        EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color); // Bottom
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color); // Left
        EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color); // Right
    }
    void Update()
    {
        if (currentMode == Mode.Place)
        {
            UpdateGhost();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                PlaceBlock();
        }

        if (currentMode == Mode.Select)
        {
            HandleDuplication();
            HandleDeletion();

            if (Mouse.current.leftButton.wasPressedThisFrame && Keyboard.current.leftCtrlKey.isPressed)
            {

                dragging = true;
                dragStart = Mouse.current.position.ReadValue();
                
                Debug.Log("DRAGGING");
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (dragging)
                {
                    dragging = false;
                    dragEnd = Mouse.current.position.ReadValue();
                    foreach (Transform transformObj in levelParent) { 
                        Vector2 objScreenPos = new Vector2(editorCamera.WorldToScreenPoint(transformObj.position).x,editorCamera.WorldToScreenPoint(transformObj.position).y);
                        

                        if (dragStart.x > dragEnd.x)
                        {
                            (dragStart.x, dragEnd.x) = (dragEnd.x, dragStart.x);
                        }
                        if (dragStart.y > dragEnd.y)
                        {
                            (dragStart.y, dragEnd.y) = (dragEnd.y, dragStart.y);
                        }
                        
                        if (objScreenPos.x > dragStart.x &&
                            objScreenPos.x < dragEnd.x &&
                            objScreenPos.y > dragStart.y &&
                            objScreenPos.y < dragEnd.y )
                        {
                          
                            GameObject go = transformObj.gameObject;
                            go.layer = 6;
                            targetObjects.Add(go);

                            UpdateGizmoPosition();
                            positionGizmo.SetActive(true);
                        }

                    }
                    
                    Debug.Log("DRAGGING STOPPED");

                   
                    
                }

            }

            else if (Mouse.current.leftButton.wasPressedThisFrame && !Keyboard.current.leftCtrlKey.isPressed)
            {
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject())
                    return;

                SelectBlock();
            }
        }
    }

    // --------------------------------------------------
    // Selection Logic (GIZMO PRIORITY)
    // --------------------------------------------------
    void SelectBlock()
    {
        Ray ray = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // 1️⃣ GIZMO RAYCAST (highest priority)
        if (Physics.Raycast(ray, out RaycastHit gizmoHit, 1000f, gizmoMask))
        {
            if (gizmoHit.collider.transform.parent.TryGetComponent(out Gizmo gizmo))
            {
                GizmoHandler.GizmoSelected(gizmo.gameObject);
                return;
            }
        }

        // 2️⃣ OBJECT RAYCAST
        if (Physics.Raycast(ray, out RaycastHit objHit, 1000f, objectMask))
        {
            if (objHit.collider.TryGetComponent(out LevelObject levelObject))
            {
                if (!Keyboard.current.leftCtrlKey.isPressed)
                    ClearSelection();

                GameObject go = levelObject.gameObject;
                go.layer = 6;
                targetObjects.Add(go);

                UpdateGizmoPosition();
                positionGizmo.SetActive(true);
                return;
            }
        }

        // 3️⃣ NOTHING HIT
        ClearSelection();
    }

    // --------------------------------------------------
    // Selection Helpers
    // --------------------------------------------------
    public void ClearSelection()
    {
        GizmoHandler.GizmoUnselected();
        positionGizmo.SetActive(false);

        foreach (GameObject obj in targetObjects)
            if (obj != null)
            {
                obj.layer = 0;
            }

        targetObjects.Clear();
    }

    void UpdateGizmoPosition()
    {
        if (targetObjects.Count == 0) return;

        Vector3 avg = Vector3.zero;
        foreach (var obj in targetObjects)
            avg += obj.transform.position;

        positionGizmo.transform.position = avg / targetObjects.Count;
    }

    // --------------------------------------------------
    // Duplication / Delete
    // --------------------------------------------------
    void HandleDuplication()
    {
        if (targetObjects.Count == 0) return;
        if (Keyboard.current.leftCtrlKey.isPressed &&
            Keyboard.current.dKey.wasPressedThisFrame)
        {

            List<GameObject> newObjects = new List<GameObject>();

            foreach (GameObject obj in targetObjects)
            {
                var dup = Instantiate(obj, obj.transform.position, obj.transform.rotation, levelParent);
                newObjects.Add(dup);
            }

           // ClearSelection();
           foreach (GameObject obj in targetObjects)
            if (obj != null)
            {
                obj.layer = 0;
            }

        targetObjects.Clear();
            foreach (var obj in newObjects)
            {
                obj.layer = 6;
                targetObjects.Add(obj);
            }

            UpdateGizmoPosition();
            positionGizmo.SetActive(true);
        }
    }

    void HandleDeletion()
    {
        if (targetObjects.Count == 0) return;
        if (!Keyboard.current.deleteKey.wasPressedThisFrame) return;

        foreach (GameObject obj in targetObjects)
            Destroy(obj);

        ClearSelection();
    }

    // --------------------------------------------------
    // Ghost / Placement
    // --------------------------------------------------
    public void UpdateSelected(GameObject obj)
    {
        selectedObject = obj;
        CreateGhost();
    }

    void CreateGhost()
    {
        if (ghost != null)
            Destroy(ghost);

        ghost = Instantiate(selectedObject);
        ghost.name = "GhostBlock";

        ghostRenderer = ghost.GetComponent<MeshRenderer>();
        ghostRenderer.sharedMaterial = ghostMaterial;

        foreach (var col in ghost.GetComponentsInChildren<Collider>())
            col.enabled = false;

        ghost.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    void UpdateGhost()
    {
        Ray ray = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
        {
            ghost.SetActive(true);

            Vector3 offset = hit.normal * (gridSize * 0.5f);
            ghost.transform.position = SnapToGrid(hit.point + offset);
        }
        else
        {
            ghost.SetActive(false);
        }
    }

    void PlaceBlock()
    {
        if (!ghost.activeSelf) return;

        Instantiate(selectedObject, ghost.transform.position, Quaternion.identity, levelParent);
    }

    // --------------------------------------------------
    // Grid
    // --------------------------------------------------
    Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            Mathf.Round(pos.y / gridSize) * gridSize,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }
}
