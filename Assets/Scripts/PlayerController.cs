using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    Vector2 MousePos;

    public GameObject rocket;
    private Rigidbody rb;
    private ConstantForce propulsion;

    public ParticleSystem particleSystem;
    public AudioSource audio;

    [Header("Movement")]
    public float forwardSpeed = 3f;   // CONSTANT Geometry Dash speed
    public float thrustForce = 1200f;    // Upward force when holding input

    private void Start()
    {
        rb = rocket.GetComponent<Rigidbody>();
        propulsion = rocket.GetComponent<ConstantForce>();

      //  rb.useGravity = false;
        rb.maxLinearVelocity = Mathf.Infinity;
    }

    void Update()
    {
        if (UIManager.IsPaused || GameManager.playerDead)
        {
            propulsion.relativeForce = Vector3.zero;
            particleSystem.enableEmission = false;
            audio.mute = true;
            return;
        }

        // Thrust (hold to go up)
     
        //Add this later to if statement
        //OptionsMenu.buttons[OptionsMenu.boostInputIndex].isPressed
        if (Mouse.current.leftButton.isPressed)
        {
            propulsion.relativeForce = new Vector3(0f, thrustForce, 0f);
            particleSystem.enableEmission = true;
            audio.mute = false;
        }
        else
        {
            propulsion.relativeForce = Vector3.zero;
            particleSystem.enableEmission = false;
            audio.mute = true;
        }

        // Rotation (your existing mouse-based rotation)
        MousePos = Mouse.current.position.ReadValue();
        float normalizedMouseX = MousePos.x / Screen.width;
        float normalizedMouseY = MousePos.y / Screen.height;
        
        Quaternion target = Quaternion.Euler(
            (1f - normalizedMouseY) * 360f - 90f,
            normalizedMouseX * 360f - 180f,
            0f
        );
       // float yaw = normalizedMouseX * 180f - 90f; // range -90 to +90
       // float pitch = (1f - normalizedMouseY) * 180f;

        // Clamp pitch if desired
      //  pitch = Mathf.Clamp(pitch, 0f, 180f);

       // Quaternion target = Quaternion.Euler(pitch, yaw, 0f);

        rocket.transform.rotation = target;
        rb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (UIManager.IsPaused || GameManager.playerDead)
            return;

        // FORCE constant forward speed (Geometry Dash behavior)
        Vector3 velocity = rb.linearVelocity;
        velocity.z = forwardSpeed;
        rb.linearVelocity = velocity;
    }
}
