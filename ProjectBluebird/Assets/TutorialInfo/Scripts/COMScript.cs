using UnityEngine;
using System;
using UnityEngine.InputSystem;

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

    public static float thrustRatio(Rotor rotor) {
        return 1.0f / (Rotor.MAXDISPLAYSPEED / rotor.speed) / (Rotor.MAXDISPLAYSPEED / Math.Abs(rotor.speed));
    }

    private void simulateMotor(Rotor rotor) {
        float thrustPower;
        if (rotor.speed == 0) {
            thrustPower = 0;
        } else {
            thrustPower = Rotor.MAXPOWER * thrustRatio(rotor);
        }
        rb.AddForceAtPosition(rotor.transform.up * thrustPower, rotor.transform.position, ForceMode.Force);
        float torque = Rotor.DRAGCOEFF * Rotor.AIRDENSITY * (float) Math.Pow((rotor.speed * Math.PI / 180) * (Rotor.MAXRPM * 6 / Rotor.MAXDISPLAYSPEED) / 60, 2) * (float) Math.Pow(Rotor.ROTORDIAMETER, 5);
        float cw = rotor.clockwise ? 1.0f : -1.0f;
        rb.AddTorque(rotor.transform.up * torque * cw);
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
