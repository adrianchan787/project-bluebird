using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Pivot : MonoBehaviour
{
    private Rigidbody rb;
    private static readonly float LOCALCENTROID = (float)(-0.8f / Math.Sqrt(3)); // CHANGEABLE
    private const float MOTORWEIGHT = 0.007f;
    private const float FRAMEWEIGHT = 0.07f;
    public Rotor leftRotor;
    public Rotor rightRotor;
    public Rotor pitchRotor;
    public Rotor thrustRotor;
    public PitchMotor pitchMotor;
    private Vector3 com = new Vector3(0, (float) 0, LOCALCENTROID);

    private Vector3 calcCOM() {
        Vector3 pitchMotorPos = pitchMotor.transform.localPosition;
        float COMY = (MOTORWEIGHT * pitchMotorPos.y) / (FRAMEWEIGHT + 4 * MOTORWEIGHT);
        float COMX = (MOTORWEIGHT * pitchMotorPos.x) / (FRAMEWEIGHT + 4 * MOTORWEIGHT);
        Vector3 COMOffset = new Vector3(COMX, COMY, 0);
        return com + COMOffset;
    }

    private void simulateMotor(Rotor rotor) {
        rb.AddForceAtPosition(transform.up * rotor.thrustRatio * Rotor.MAXPOWER, rotor.transform.position, ForceMode.Force);
        rb.AddTorque(transform.up * rotor.torque);
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = com;
    }

    void FixedUpdate()
    {
        simulateMotor(leftRotor);
        simulateMotor(rightRotor);
        simulateMotor(pitchRotor);        
        simulateMotor(thrustRotor);
    }
}
