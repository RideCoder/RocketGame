using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditorController : MonoBehaviour
{
    public Camera editorCamera;
    public Transform levelParent;
    public GameObject blockPrefab;

    public float gridSize = 1f;
    public LayerMask placementMask;
    public Material ghostMaterial;
    GameObject ghost;

    void Start()
    {
        CreateGhost();
    }

    void Update()
    {
        UpdateGhost();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceBlock();
        }
    }

    void CreateGhost()
    {
        ghost = Instantiate(blockPrefab);
        ghost.name = "GhostBlock";
        ghost.GetComponent<MeshRenderer>().sharedMaterial = ghostMaterial;
     
        // Disable collisions
        foreach (var col in ghost.GetComponentsInChildren<Collider>())
            col.enabled = false;

       
    }

    void UpdateGhost()
    {
        Ray ray = editorCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementMask))
        {
            ghost.SetActive(true);
            ghost.transform.position = SnapToGrid(hit.point);
        }
        else
        {
            ghost.SetActive(false);
        }
    }

    void PlaceBlock()
    {
        if (!ghost.activeSelf) return;

        Vector3 pos = ghost.transform.position;
        GameObject obj = Instantiate(blockPrefab, pos, Quaternion.identity, levelParent);
        obj.GetComponent<LevelObject>().type = "Block";
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            Mathf.Round(pos.y / gridSize) * gridSize,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }
}
