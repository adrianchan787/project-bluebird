using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Pivot : MonoBehaviour
{
    // Rigidbody for physics
    private Rigidbody rb;

    // COM calculations
    private static readonly float LOCALCENTROID = (float)(-0.8f / Math.Sqrt(3)); // CHANGEABLE
    private const float MOTORWEIGHT = 0.007f;
    private const float FRAMEWEIGHT = 0.07f;
    public static float FRAMEHEIGHT = 0.525f;
    private Vector3 com = new Vector3(0, (float) 0, LOCALCENTROID);

    // Rotors
    public Rotor leftRotor;
    public Rotor rightRotor;
    public Rotor pitchRotor;
    public Rotor thrustRotor;

    // Servo
    public PitchMotor pitchMotor;

    // Data
    public Vector3 pos;
    public Vector3 vel;
    public Vector3 accel;
    public Quaternion orientation;
    public Vector3 aVel;
    public Vector3 aAccel;

    private Vector3 calcCOM() {
        Vector3 pitchMotorPos = pitchMotor.transform.localPosition;
        float COMY = (MOTORWEIGHT * pitchMotorPos.y) / (FRAMEWEIGHT + 4 * MOTORWEIGHT);
        float COMX = (MOTORWEIGHT * pitchMotorPos.x) / (FRAMEWEIGHT + 4 * MOTORWEIGHT);
        Vector3 COMOffset = new Vector3(COMX, COMY, 0);
        return com + COMOffset;
    }

    private void simulateMotor(Rotor rotor) {
        rb.AddForceAtPosition(rotor.transform.up * rotor.thrustRatio * Rotor.MAXPOWER, rotor.transform.position, ForceMode.Force);
        rb.AddTorque(rotor.transform.up * rotor.torque);
    }

    private void calculateLinear() {
        Vector3 temp = rb.linearVelocity;
        Vector3 velDelta = temp - vel;
        accel = velDelta / Time.fixedDeltaTime;
        vel = temp;
    }

    private void calculateAngular() {
        Vector3 temp = rb.angularVelocity;
        Vector3 velDelta = temp - aVel;
        aAccel = velDelta / Time.fixedDeltaTime;
        aVel = temp;
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = com;
        vel = rb.linearVelocity;
        aVel = rb.angularVelocity;
    }

    void FixedUpdate()
    {
        simulateMotor(leftRotor);
        simulateMotor(rightRotor);
        simulateMotor(pitchRotor);        
        simulateMotor(thrustRotor);
        pos = rb.position;
        orientation = rb.rotation;
        calculateLinear();
        calculateAngular();
    }
}
