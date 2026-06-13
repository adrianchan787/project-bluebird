using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Pivot : MonoBehaviour
{
    // Rigidbody for physics
    private Rigidbody rb;

    // COM calculations
    private static readonly float LOCALCENTROID = (float)(-0.08f / Math.Sqrt(3)); // CHANGEABLE
    private const float MOTORWEIGHT = 0.007f;
    private const float FRAMEWEIGHT = 0.06f;
    public static float FRAMEHEIGHT = 0.0525f;
    public Vector3 com = new Vector3(0, (float) 0, LOCALCENTROID);

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

    public float pitch;
    public float yaw;
    public float roll;
    public Vector3 aVel;
    public Vector3 aAccel;

    private Vector3 calcCOM() {
        Vector3 pitchMotorPos = pitchMotor.transform.localPosition; // Position Relative to drone
        float COMY = (MOTORWEIGHT * pitchMotorPos.y) / (FRAMEWEIGHT + 4 * MOTORWEIGHT);
        float COMX = (MOTORWEIGHT * pitchMotorPos.x) / (FRAMEWEIGHT + 4 * MOTORWEIGHT);
        Vector3 COMOffset = new Vector3(COMX, COMY, 0);
        return com + COMOffset;
    }

    private static Vector3 calculateOrientation(Quaternion q)
    {
        Vector3 rawEuler = q.eulerAngles;
        float pitch = FlightController.NormalizeAngle(rawEuler.x); 
        float roll = FlightController.NormalizeAngle(rawEuler.z); 
        Vector3 forwardVector = q * Vector3.forward;
        float yaw = Mathf.Atan2(forwardVector.x, forwardVector.z) * Mathf.Rad2Deg;
        yaw = FlightController.NormalizeAngle(yaw);

        return new Vector3(-pitch, yaw, roll);
    }

    private void calculateLocalAngles()
    {
        // Project the drone's local forward vector onto the global horizontal plane to get true pitch
        Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        pitch = -Vector3.SignedAngle(forwardOnPlane, transform.forward, transform.right);

        // Project the drone's local right vector onto the horizontal plane to get true roll
        Vector3 rightOnPlane = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        roll = Vector3.SignedAngle(rightOnPlane, transform.right, transform.forward);
        
        // For yaw, use the delta angle around the world up axis to prevent pitch coupling
        yaw = FlightController.NormalizeAngle(transform.eulerAngles.y); 
    }

    private void simulateMotor(Rotor rotor) {
        rb.AddForceAtPosition(rotor.transform.up * rotor.thrustRatio * Rotor.MAXPOWER, rotor.transform.position, ForceMode.Force);
        rb.AddTorque(rotor.transform.up * rotor.torque);
        Debug.Log($"Rotor Torque: {rotor.torque}");
    }

    private void calculateLinear() {
        Vector3 temp = transform.InverseTransformDirection(rb.linearVelocity);
        temp.x = -temp.x;
        Vector3 velDelta = temp - vel;
        accel = velDelta / Time.fixedDeltaTime;
        vel = temp;
    }

    private void calculateAngular() {
        Vector3 temp = transform.InverseTransformDirection(rb.angularVelocity);
        temp.x = -temp.x;
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
        Vector3 orientation = calculateOrientation(rb.rotation);
        calculateLocalAngles();
        //pitch = orientation.x;
        //yaw = orientation.y;
        //roll = orientation.z;
        calculateLinear();
        calculateAngular();
        Vector3 totalInputTorque = rb.GetAccumulatedTorque();

    }
}
