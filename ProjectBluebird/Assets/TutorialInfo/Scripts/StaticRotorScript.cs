using UnityEngine;
using UnityEngine.InputSystem;

public class Rotor : MonoBehaviour
{
    public const float MAXDISPLAYSPEED = 1080.0f; // In Deg/s
    public static float MAXRPM = 15000; 
    public static float MAXPOWER = 0.25f; // In Newtons
    public static float DRAGCOEFF = 0.6f; // For Props, dimensionless
    public static float AIRDENSITY = 1.225f;
    public static float ROTORDIAMETER = 0.08f; // rotor diameter, in cm, purely for torque calculation purposes
    public static float INCREMENT = 3f;
    public static float DECREMENT = 0.5f;
    public float debugThrust = 0;

    public float speed = 0f; // In Deg/s
    public bool clockwise = true; // 1 if clockwise, -1 if not
    private float cw;
    public InputActionReference pRot;
    public InputActionReference nRot;

    public static float Clamp(float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    void Start() {
        pRot.action.Enable();
        nRot.action.Enable();
        cw = clockwise ? 1.0f : -1.0f;
    }

    void Update()
    {
 
        if (Keyboard.current != null)
        {
            if (pRot.action.IsPressed()) { 
                speed += INCREMENT;
            } else if (nRot.action.IsPressed()) {
                speed -= DECREMENT;
            }
        }

        speed = Clamp(speed, -MAXDISPLAYSPEED, MAXDISPLAYSPEED);
        if (speed == 0) {
            debugThrust = 0;
        } else {
            debugThrust = Rotor.MAXPOWER / (Rotor.MAXDISPLAYSPEED / speed) / (Rotor.MAXDISPLAYSPEED / speed);
        }
        
        transform.Rotate(0, speed * Time.deltaTime * cw, 0, Space.Self);

    }
}
