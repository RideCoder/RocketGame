using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


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
    public bool autoRotate = true;
    public TMP_Text autoRotateText;

    public TMP_Text selectText;
    public TMP_Text placeText;
    public RectTransform selection;
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

    // 🔔 EVENT (parameterless)
    public static event Action OnTargetObjectsUpdated;

    private static void NotifyTargetObjectsUpdated()
    {
        OnTargetObjectsUpdated?.Invoke();
    }

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
        Debug.Log(positionGizmo);
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
  
        LoadObjectList.OnObjectSelected -= UpdateSelected;
    
    }

    // --------------------------------------------------
    // Mode Switching
    // --------------------------------------------------
    public void SetSelectMode()
    {
        ghost.SetActive(false);
        currentMode = Mode.Select;
        selectText.color = new UnityEngine.Color(60f / 255f, 93f / 255f, 255, 255);
        placeText.color = UnityEngine.Color.white;
    }
    public void ToggleAutoRotate()
    {
        autoRotate = !autoRotate;
        if (autoRotate)
        {
            autoRotateText.color = new UnityEngine.Color(0f, 1f, 0f);
        }
        else
        {
            ghost.transform.rotation = Quaternion.identity;
            autoRotateText.color = new UnityEngine.Color(1f, 0f, 0f);
        }
    }
    public void SetPlaceMode()
    {
        GizmoHandler.GizmoUnselected();
        positionGizmo.SetActive(false);

        foreach (GameObject obj in targetObjects)
            if (obj != null)
                obj.layer = obj.GetComponent<LevelObject>().mask;

        targetObjects.Clear();
        NotifyTargetObjectsUpdated();

        currentMode = Mode.Place;

        selectText.color = UnityEngine.Color.white;
        placeText.color = new UnityEngine.Color(60f / 255f, 93f / 255f, 255, 255);
    }

    // --------------------------------------------------
    // Unity Lifecycle
    // --------------------------------------------------
    void Start()
    {
        LoadObjectList.OnObjectSelected += UpdateSelected;
        selectedObject = startObject;
        Debug.Log("TESt");
        CreateGhost();
        dragging = false;
        targetObjects.Clear();
        NotifyTargetObjectsUpdated();

        GizmoHandler.OnModeSwitch +=  UpdateGizmoPosition;


        if (currentMode == Mode.Select)
        {
            selectText.color = new UnityEngine.Color(60f / 255f, 93f / 255f, 255, 255);


        }
        else if (currentMode == Mode.Place)
        {
            placeText.color = new UnityEngine.Color(60f / 255f, 93f / 255f, 255, 255);
        }
    }

    void OnGUI()
    {
        if (!dragging) return;

        Vector2 currentMouse = Mouse.current.position.ReadValue();

        float xMin = Mathf.Min(dragStart.x, currentMouse.x);
        float yMin = Screen.height - Mathf.Max(dragStart.y, currentMouse.y);
        float width = Mathf.Abs(dragStart.x - currentMouse.x);
        float height = Mathf.Abs(dragStart.y - currentMouse.y);

        Rect rect = new Rect(xMin, yMin, width, height);
        DrawRectBorder(rect, 2f, UnityEngine.Color.cyan);
    }

    static Texture2D _whiteTex;

    void DrawRectBorder(Rect rect, float thickness, UnityEngine.Color color)
    {
        if (_whiteTex == null)
        {
            _whiteTex = new Texture2D(1, 1);
            _whiteTex.SetPixel(0, 0, UnityEngine.Color.white);
            _whiteTex.Apply();
        }

        UnityEngine.Color oldColor = GUI.color;
        GUI.color = color;

        // Top
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), _whiteTex);
        // Bottom
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), _whiteTex);
        // Left
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), _whiteTex);
        // Right
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), _whiteTex);

        GUI.color = oldColor;
    }



    void Update()
    {
        if (currentMode == Mode.Place)
        {
            UpdateGhost();
            if (Mouse.current.leftButton.wasPressedThisFrame)
                PlaceBlock();
        }

        if (currentMode != Mode.Select)
            return;

        HandleDuplication();
        HandleDeletion();

        if (Mouse.current.leftButton.wasPressedThisFrame && Keyboard.current.leftCtrlKey.isPressed)
        {
            dragging = true;
            dragStart = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && dragging)
        {
            dragging = false;
            dragEnd = Mouse.current.position.ReadValue();
            if (dragStart != dragEnd)
            {
                foreach (Transform t in levelParent)
                {
                    Vector2 screenPos = editorCamera.WorldToScreenPoint(t.position);

                    if (screenPos.x > Mathf.Min(dragStart.x, dragEnd.x) &&
                        screenPos.x < Mathf.Max(dragStart.x, dragEnd.x) &&
                        screenPos.y > Mathf.Min(dragStart.y, dragEnd.y) &&
                        screenPos.y < Mathf.Max(dragStart.y, dragEnd.y))
                    {
                        if (!targetObjects.Contains(t.gameObject))
                        {
                            t.gameObject.layer = 6;
                            targetObjects.Add(t.gameObject);
                        }
                    }
                }
            }
            else
            {
                SelectBlock();
            }

                UpdateGizmoPosition();
            positionGizmo.SetActive(targetObjects.Count > 0);
            NotifyTargetObjectsUpdated();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame &&
            !Keyboard.current.leftCtrlKey.isPressed)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            SelectBlock();
        }
    }

    // --------------------------------------------------
    // Selection Logic
    // --------------------------------------------------
    void SelectBlock()
    {
        Ray ray = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit gizmoHit, 1000f, gizmoMask))
        {
            if (gizmoHit.collider.transform.parent.TryGetComponent(out Gizmo gizmo))
            {
                GizmoHandler.GizmoSelected(gizmo.gameObject);
                return;
            }
        }

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, objectMask))
        {
            if (hit.collider.TryGetComponent(out LevelObject levelObject))
            {
                if (!Keyboard.current.leftCtrlKey.isPressed)
                {

                    ClearSelection();
                }
               

                GameObject go = levelObject.gameObject;
                go.layer = 6;
                targetObjects.Add(go);

                UpdateGizmoPosition();
                positionGizmo.SetActive(true);
                NotifyTargetObjectsUpdated();
                return;
            }
        }
        if (!Keyboard.current.leftCtrlKey.isPressed)
        {
           
            ClearSelection();
        }
        GizmoHandler.GizmoUnselected();
    }

    // --------------------------------------------------
    // Selection Helpers
    // --------------------------------------------------
    public void ClearSelection()
    {
      //  GizmoHandler.GizmoUnselected();
        positionGizmo.SetActive(false);

        foreach (GameObject obj in targetObjects)
            if (obj != null)
                obj.layer = obj.GetComponent<LevelObject>().mask;

        targetObjects.Clear();
        NotifyTargetObjectsUpdated();
    }

    void UpdateGizmoPosition(int mode = 0)
    {
        if (targetObjects.Count == 0) return;

        Vector3 avg = Vector3.zero;
        foreach (var obj in targetObjects)
        {
            Debug.Log(obj);
            Debug.Log(targetObjects.Count);
            Debug.Log(positionGizmo);
            if (obj != null)
            {

                avg += obj.transform.position;
            }
        }
        if (positionGizmo != null)
        {
            if (targetObjects.Count > 0)
            {
                positionGizmo.transform.position = avg / targetObjects.Count;
            }

            if (mode == 1 && targetObjects[0] != null)
            {
                positionGizmo.transform.rotation = targetObjects[0].transform.rotation;
            }
        }
    }

    // --------------------------------------------------
    // Duplication / Delete
    // --------------------------------------------------
    void HandleDuplication()
    {
        if (!Keyboard.current.leftCtrlKey.isPressed ||
            !Keyboard.current.dKey.wasPressedThisFrame ||
            targetObjects.Count == 0)
            return;

        List<GameObject> newObjects = new List<GameObject>();

        foreach (GameObject obj in targetObjects)
        {
     
            newObjects.Add(Instantiate(obj, obj.transform.position, obj.transform.rotation, levelParent));

        }
  

        foreach (var obj in newObjects)
        {
            obj.layer = obj.GetComponent<LevelObject>().mask; 
     
        }

        UpdateGizmoPosition();
        positionGizmo.SetActive(true);
        NotifyTargetObjectsUpdated();
    }

    void HandleDeletion()
    {
        if (!Keyboard.current.deleteKey.wasPressedThisFrame ||
            targetObjects.Count == 0)
            return;

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
            if (GizmoHandler.snapEnabled)
            {
                ghost.transform.position = SnapToGrid(hit.point + offset);
            }
            else
            {
                ghost.transform.position = hit.point + offset;
            }


            // Align spike's Y+ axis with the surface normal
            if (selectedObject.GetComponent<LevelObject>().SurfaceNormalRotation && autoRotate)
            {
                ghost.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
        }
        else
        {
            ghost.SetActive(false);
        }
    }

    void PlaceBlock()
    {
        if (!ghost.activeSelf) return;
        if (selectedObject.GetComponent<LevelObject>().SurfaceNormalRotation && autoRotate)
        {
            Instantiate(selectedObject, ghost.transform.position, ghost.transform.rotation, levelParent);
        }
        else
        {
            Instantiate(selectedObject, ghost.transform.position, Quaternion.identity, levelParent);
        }
        
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
