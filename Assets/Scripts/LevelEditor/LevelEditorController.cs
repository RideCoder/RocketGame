using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditorController : MonoBehaviour
{
    [Header("References")]
    public Camera editorCamera;
    public Transform levelParent;
    public static GameObject selectedObject;
    public GameObject startObject;

    [Header("Placement Settings")]
    public float gridSize = 1f;
    public LayerMask placementMask;

    [Header("Ghost Settings")]
    public Material ghostMaterial;

    GameObject ghost;
    MeshRenderer ghostRenderer;
    public GameObject positionGizmo;
    public static GameObject targetObject;
    

    public enum Mode
    {
        Select,
        Place
    }

    public Mode currentMode;

    public void SetSelectMode()
    {
        ghost.SetActive(false);
        currentMode = Mode.Select;
    }

    public void SetPlaceMode()
    {
        currentMode = Mode.Place;
    }


    void Start()
    {

        LoadObjectList.OnObjectSelected += UpdateSelected;
        selectedObject = startObject;
            CreateGhost();
        
       
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

           
            if (Mouse.current.leftButton.wasPressedThisFrame)
                SelectBlock();
        }

    }

    void SelectBlock()
    {
       

        
        Ray ray = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
        {
            if (hit.collider.gameObject.transform.parent.TryGetComponent(out Gizmo gizmo))
            {
                Debug.Log(hit.collider.gameObject.transform.parent.name);
                Debug.Log(gizmo.name);
                GizmoHandler.GizmoSelected(gizmo.gameObject);
                return;
            }
            if (hit.collider.gameObject.TryGetComponent(out LevelObject levelObject))
            {
                positionGizmo.transform.position = levelObject.gameObject.transform.position;
                positionGizmo.SetActive(true);
                targetObject = levelObject.gameObject;
                Debug.Log(levelObject.transform.position);
            }
            else
            {
                GizmoHandler.GizmoUnselected();
                positionGizmo.SetActive(false);
                targetObject = null;
            }

        }
    }

    public void UpdateSelected(GameObject obj)
    {
       
        selectedObject = obj;
        CreateGhost();
    }

    void CreateGhost()
    {
        ghost = Instantiate(selectedObject);
        ghost.name = "GhostBlock";

        ghostRenderer = ghost.GetComponent<MeshRenderer>();
        ghostRenderer.sharedMaterial = ghostMaterial;

        foreach (var col in ghost.GetComponentsInChildren<Collider>())
            col.enabled = false;
        
        ghost.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    // -------------------------------
    // Ghost Placement Logic
    // -------------------------------
    void UpdateGhost()
    {
        Ray ray = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
        {
            ghost.SetActive(true);

            // Offset by half a block along surface normal
            Vector3 offset = hit.normal * (gridSize * 0.5f);

            // Apply grid snapping AFTER offset
            Vector3 snappedPosition = SnapToGrid(hit.point + offset);
            ghost.transform.position = snappedPosition;
        }
        else
        {
            ghost.SetActive(false);
        }
    }

    // -------------------------------
    // Block Placement
    // -------------------------------
    void PlaceBlock()
    {
        if (!ghost.activeSelf) return;

        Vector3 pos = ghost.transform.position;

        Instantiate(selectedObject, pos, Quaternion.identity, levelParent);
    }

    // -------------------------------
    // Grid Snapping
    // -------------------------------
    Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            Mathf.Round(pos.y / gridSize) * gridSize,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }
}
