using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 MousePos;

    [Header("References")]
    public GameObject rocket;
    public ParticleSystem particleSystem;
    public AudioSource audio;

    [Header("Movement")]
    public float forwardSpeed = 3f;      // Constant GD speed
    public float thrustForce = 1200f;    // Local-space thrust

    public Rigidbody rb;
    ConstantForce propulsion;

    bool thrusting;

    void Start()
    {
        rb = rocket.GetComponent<Rigidbody>();
        propulsion = rocket.GetComponent<ConstantForce>();

        rb.maxLinearVelocity = Mathf.Infinity;
    }

    void Update()
    {
        if (UIManager.IsPaused || GameManager.playerDead)
        {
            thrusting = false;
            particleSystem.enableEmission = false;
            audio.mute = true;
            return;
        }

        // INPUT (frame-based is fine)
        thrusting = Mouse.current.leftButton.isPressed;

        // Visuals
        particleSystem.enableEmission = thrusting;
        audio.mute = !thrusting;

        // Rotation (visual & orientation)
        MousePos = Mouse.current.position.ReadValue();
        float nx = MousePos.x / Screen.width;
        float ny = MousePos.y / Screen.height;

        rocket.transform.rotation = Quaternion.Euler(
            (1f - ny) * 360f - 90f,
            nx * 360f - 180f,
            0f
        );
    }

    void FixedUpdate()
    {
        if (UIManager.IsPaused || GameManager.playerDead)
        {
            propulsion.relativeForce = Vector3.zero;
            return;
        }

        // LOCAL-space thrust (same behavior as before)
        propulsion.relativeForce = thrusting
            ? new Vector3(0f, thrustForce, 0f)
            : Vector3.zero;

        // Constant forward speed
        Vector3 v = rb.linearVelocity;
        v.z = forwardSpeed;
        rb.linearVelocity = v;

        // Kill angular drift
        rb.angularVelocity = Vector3.zero;
    }
}
