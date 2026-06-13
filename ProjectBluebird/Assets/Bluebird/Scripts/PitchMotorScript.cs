using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PitchMotor : MonoBehaviour
{

    private const float DEG_PER_UPDATE = 1f;
    public float direction = 0.0f; // Direction of pitchmotor in degrees, 0 = points global up

    private void changeAngle(float angle)
    {
        // TODO use the normalize function
        float orientation = transform.eulerAngles.z;
        if (orientation > 180f) 
        {
            orientation -= 360f;
        }
        float angleDifference = angle - orientation;
        if (Math.Abs(angleDifference) > DEG_PER_UPDATE)
        {
            if (angleDifference > 0)
            {
                transform.Rotate(0, 0, DEG_PER_UPDATE, Space.Self);
            } else
            {
                transform.Rotate(0, 0, -DEG_PER_UPDATE, Space.Self);
            }
        } else                                                                                        
        {
            transform.Rotate(0, 0, angleDifference, Space.Self);
        }
    }
    void FixedUpdate()
    {
        changeAngle(direction);
    }
}
