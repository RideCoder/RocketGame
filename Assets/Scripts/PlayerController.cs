using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{

    Vector2 MousePos;
    public GameObject rocket;
    private ConstantForce propulsion;
    public ParticleSystem particleSystem;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame

    private void Start()
    {
        propulsion = rocket.GetComponent<ConstantForce>();
        rocket.GetComponent<Rigidbody>().maxLinearVelocity = 30f;
    }

    public void Update()
    {
        if (!GameManager.playerDead)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                propulsion.relativeForce = new Vector3(0f, 30f, 0f);
                particleSystem.enableEmission = true;


            }
            else
            {
                propulsion.relativeForce = new Vector3(0f, 0f, 0f);
                particleSystem.enableEmission = false;

            }

            /*   if (Keyboard.current.rKey.isPressed)
               {
                   rocket.transform.position = new Vector3(.5f, 4.35f, -6.5f);
                   rocket.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
               }
            */
        }
        if (GameManager.playerDead)
        {
            propulsion.relativeForce = new Vector3(0f, 0f, 0f);
            particleSystem.enableEmission = false;
        }
    }

    void FixedUpdate()
    {

        MousePos = Mouse.current.position.ReadValue();
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        float normalizedMouseX = MousePos.x / screenWidth;
        float normalizedMouseY = MousePos.y / screenHeight;

        Quaternion target = Quaternion.Euler((1f - normalizedMouseY * 360f) - 90f, ((normalizedMouseX) * 360f) - 180f, 0f);
        //  if (Mouse.current.rightButton.isPressed)
        if (!GameManager.playerDead)
        {
            rocket.transform.rotation = target;
            rocket.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
        }



    }

   /* private void OnGUI()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        float normalizedMouseX = MousePos.x / screenWidth;
        float normalizedMouseY = MousePos.y / screenHeight;
        GUI.Label(new Rect(10, 10, 200, 100), "X: " + normalizedMouseX.ToString() + " Y: " + normalizedMouseY.ToString());
    }*/

  
}
