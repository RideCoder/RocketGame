using UnityEngine;
using UnityEngine.InputSystem;

public class EditorCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 0.15f;
  

    public float minPitch = -80f;
    public float maxPitch = 80f;

    Camera cam;
    float yaw;
    float pitch;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
    }

    void Update()
    {
        HandleKeyboardMove();
        HandleMouseRotate();
       
    }

    void HandleKeyboardMove()
    {
        Vector3 dir = Vector3.zero;
        float multiplier = 1f;

        // Horizontal movement (relative to rotation)
        if (Keyboard.current.wKey.isPressed) dir += transform.forward;
        if (Keyboard.current.sKey.isPressed) dir -= transform.forward;
        if (Keyboard.current.aKey.isPressed) dir -= transform.right;
        if (Keyboard.current.dKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
            dir += transform.right;

        // Vertical movement (world space)
        if (Keyboard.current.spaceKey.isPressed)
            dir += Vector3.up;

        if (Keyboard.current.leftShiftKey.isPressed)
            dir += Vector3.down;

        transform.position += dir * moveSpeed * Time.deltaTime * multiplier;
    }


    void HandleMouseRotate()
    {
        if (!Mouse.current.rightButton.isPressed)
            return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        yaw += delta.x * rotateSpeed;
        pitch -= delta.y * rotateSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

 
}
