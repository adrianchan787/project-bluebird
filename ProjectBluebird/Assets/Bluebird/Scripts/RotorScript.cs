using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Rotor : MonoBehaviour
{

    // "Raw" Data
    public float speed = 0f; // In Deg/s
    private float cw;

    // Motor Config
    public bool clockwise = true; // 1 if clockwise, -1 if not, set in individual instance

    public const float MAXDISPLAYSPEED = 1080.0f; // In Deg/s
    public static float MAXRPM = 15000; 
    public static float MAXPOWER = 0.35f; // In Newtons
    public static float DRAGCOEFF = 0.6f; // For Props, dimensionless
    public static float AIRDENSITY = 1.225f;
    public static float ROTORDIAMETER = 0.08f; // rotor diameter, in cm, purely for torque calculation purposes
    public static float INCREMENT = 3f;
    public static float DECREMENT = 0.5f;

    // Interface
    public float thrustRatio = 0;
    public float torque = 0;

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
        torque *= -cw;
    }

    void Start() {
        cw = clockwise ? 1.0f : -1.0f;
    }

    void Update()
    {
        motorForces();
        speed = FlightController.Clamp(speed, -MAXDISPLAYSPEED, MAXDISPLAYSPEED);
        transform.Rotate(0, speed * Time.deltaTime * cw, 0, Space.Self);

    }
}
