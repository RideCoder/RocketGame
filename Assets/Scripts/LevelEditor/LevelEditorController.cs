using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditorController : MonoBehaviour
{
    [Header("References")]
    public Camera editorCamera;
    public Transform levelParent;
    public GameObject blockPrefab;

    [Header("Placement Settings")]
    public float gridSize = 1f;
    public LayerMask placementMask;

    [Header("Ghost Settings")]
    public Material ghostMaterial;

    GameObject ghost;
    MeshRenderer ghostRenderer;

    void Start()
    {
        CreateGhost();
    }

    void Update()
    {
        UpdateGhost();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            PlaceBlock();
    }

    // -------------------------------
    // Ghost Creation
    // -------------------------------
    void CreateGhost()
    {
        ghost = Instantiate(blockPrefab);
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

        Instantiate(blockPrefab, pos, Quaternion.identity, levelParent)
            .GetComponent<LevelObject>().type = "Block";
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
