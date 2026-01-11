using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GizmoHandler : MonoBehaviour
{
    public static GameObject gizmoSelected;

    public int mode = 0; // 0=move, 1=scale, 2=rotate

    [Header("Snapping")]
    public bool enableSnapping = true;
    public float positionSnap = 1f;
    public float scaleSnap = 1f;
    public float rotationSnapDegrees = 15f;

    [Header("Sensitivity")]
    public float moveSensitivity = 1f;         // world units per pixel-ish
    public float scaleSensitivity = 1f;        // scale units per pixel-ish
    public float rotateSensitivity = 180f;     // degrees per second per pixel-ish

    private bool dragging = false;

    // Accumulated (continuous) delta for the active operation (in "gizmo space": 1D along active axis)
    private float dragAccum = 0f;

    // Cache per-object start transforms (so snapping is stable)
    private readonly Dictionary<Transform, Vector3> startPos = new();
    private readonly Dictionary<Transform, Vector3> startScale = new();
    private readonly Dictionary<Transform, Quaternion> startRot = new();

    public void ChangeMode(int m) => mode = m;

    public static void GizmoSelected(GameObject gizmo)
    {
        gizmoSelected = gizmo;
    }

    public static void GizmoUnselected()
    {
        gizmoSelected = null;
    }

    void Update()
    {
        if (gizmoSelected == null)
            return;

        // Keep gizmo centered on selection
        Vector3 avgPos = Vector3.zero;
        foreach (GameObject obj in LevelEditorController.targetObjects)
            avgPos += obj.transform.position;

        avgPos /= Mathf.Max(1, LevelEditorController.targetObjects.Count);
        gizmoSelected.transform.parent.position = avgPos;

        bool snappingActive =
            enableSnapping &&
            !(Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed);

        // Begin drag on press
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            BeginDrag();
        }

        // Dragging
        if (Mouse.current.leftButton.isPressed && dragging)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            dragAccum += MouseDeltaToAxisAmount(mouseDelta, mode);

            // Compute snapped 1D delta
            float snapped = snappingActive ? Snap1D(dragAccum, GetSnapForMode(mode)) : dragAccum;

            // Apply to all selected objects based on their start transforms
            foreach (GameObject go in LevelEditorController.targetObjects)
            {
                Transform t = go.transform;

                if (mode == 0) // MOVE
                {
                    Vector3 axis = GetAxisVector(gizmoSelected.name);
                    t.position = startPos[t] + axis * snapped;
                }
                else if (mode == 1) // SCALE
                {
                    Vector3 s = startScale[t];
                    if (gizmoSelected.name == "GizmoX") s.x = startScale[t].x + snapped;
                    if (gizmoSelected.name == "GizmoY") s.y = startScale[t].y + snapped;
                    if (gizmoSelected.name == "GizmoZ") s.z = startScale[t].z + snapped;
                    t.localScale = s;
                }
                else if (mode == 2) // ROTATE
                {
                    Vector3 axis = GetAxisVector(gizmoSelected.name);
                    // snapped here is degrees (see MouseDeltaToAxisAmount for rotate)
                    t.rotation = Quaternion.AngleAxis(snapped, axis) * startRot[t];
                }
            }
        }

        // End drag on release
        if (Mouse.current.leftButton.wasReleasedThisFrame && dragging)
        {
            EndDrag();
            GizmoUnselected();
        }
    }

    private void BeginDrag()
    {
        dragging = true;
        dragAccum = 0f;

        startPos.Clear();
        startScale.Clear();
        startRot.Clear();

        foreach (GameObject go in LevelEditorController.targetObjects)
        {
            Transform t = go.transform;
            startPos[t] = t.position;
            startScale[t] = t.localScale;
            startRot[t] = t.rotation;
        }
    }

    private void EndDrag()
    {
        dragging = false;
        dragAccum = 0f;

        startPos.Clear();
        startScale.Clear();
        startRot.Clear();
    }

    private float GetSnapForMode(int m)
    {
        if (m == 0) return Mathf.Max(0.0001f, positionSnap);
        if (m == 1) return Mathf.Max(0.0001f, scaleSnap);
        return Mathf.Max(0.0001f, rotationSnapDegrees);
    }

    private float Snap1D(float value, float snap)
    {
        return Mathf.Round(value / snap) * snap;
    }

    private Vector3 GetAxisVector(string gizmoName)
    {
        // World axes, matching your original script behavior.
        if (gizmoName == "GizmoX") return Vector3.right;
        if (gizmoName == "GizmoY") return Vector3.up;
        return Vector3.forward; // GizmoZ
    }

    private float MouseDeltaToAxisAmount(Vector2 mouseDelta, int m)
    {
        // Convert mouse delta into a 1D amount along the selected gizmo axis.
        // This preserves your original “X uses delta.x, Y uses delta.y, Z uses delta.x” feel,
        // but now it feeds a snapped accumulator.
        float raw;

        if (gizmoSelected.name == "GizmoY")
            raw = mouseDelta.y;
        else
            raw = mouseDelta.x;

        // Scale to “per second” so FPS doesn’t change feel
        float dt = Mathf.Max(0.0001f, Time.deltaTime);

        if (m == 0) return raw * dt * moveSensitivity;
        if (m == 1) return raw * dt * scaleSensitivity;
        /* m == 2 */
        return raw * dt * rotateSensitivity; // degrees
    }
}
