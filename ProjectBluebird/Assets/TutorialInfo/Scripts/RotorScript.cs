using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Rotor : MonoBehaviour
{

    // "Raw" Data
    private float speed = 0f; // In Deg/s
    private float cw;

    // Motor Config
    public bool clockwise = true; // 1 if clockwise, -1 if not, set in individual instance

    public const float MAXDISPLAYSPEED = 1080.0f; // In Deg/s
    public static float MAXRPM = 15000; 
    public static float MAXPOWER = 0.25f; // In Newtons
    public static float DRAGCOEFF = 0.6f; // For Props, dimensionless
    public static float AIRDENSITY = 1.225f;
    public static float ROTORDIAMETER = 0.08f; // rotor diameter, in cm, purely for torque calculation purposes
    public static float INCREMENT = 3f;
    public static float DECREMENT = 0.5f;

    // Controls
    public InputActionReference pRot;
    public InputActionReference nRot;

    // Interface
    public float thrustRatio = 0;
    public float torque = 0;

    private static float Clamp(float value, float min, float max) {
        return (value < min) ? min : (value > max) ? max : value;
    }

    private float calculateThrustRatio() {
        return 1.0f / (MAXDISPLAYSPEED / speed) / (MAXDISPLAYSPEED / Math.Abs(speed));
    }

    private void motorForces() {

        // Thrust Calcs
        if (speed == 0) {
            thrustRatio = 0;
        } else {
            thrustRatio = calculateThrustRatio();
        }

        // Torque Calcs
        torque = DRAGCOEFF * AIRDENSITY * (float) Math.Pow((speed * Math.PI / 180) * (MAXRPM * 6 / MAXDISPLAYSPEED) / 60, 2) * (float) Math.Pow(ROTORDIAMETER, 5);
        torque *= cw;
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
                speed = 0;
            }
        }

        motorForces();
        speed = Clamp(speed, -MAXDISPLAYSPEED, MAXDISPLAYSPEED);
        transform.Rotate(0, speed * Time.deltaTime * cw, 0, Space.Self);

    }
}
