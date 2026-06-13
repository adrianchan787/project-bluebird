using UnityEngine;
using TMPro;
using System;

public class UIScript : MonoBehaviour {
    public Pivot body;

    [SerializeField] private TMP_InputField LT, RT, PT, TT, VV, VA, Yaw, Pitch, Roll, AVYaw, AVPitch, AVRoll, AAYaw, AAPitch, AARoll, PosX, PosY, ALT, GRSP, HEAD, SDEG;


    private void updateDashboard(Pivot pivot) {

        // Thrust Levels
        float LTval = 100f * pivot.leftRotor.thrustRatio;
        float RTval = 100f * pivot.rightRotor.thrustRatio;
        float PTval = 100f * pivot.pitchRotor.thrustRatio;
        float TTval = 100f * pivot.thrustRotor.thrustRatio;
        float S_deg = pivot.pitchMotor.direction;
        LT.text = LTval.ToString("F4");
        RT.text = RTval.ToString("F4");
        PT.text = PTval.ToString("F4");
        TT.text = TTval.ToString("F4");
        SDEG.text = S_deg.ToString("F4");

        // Orientation
        float pitch = pivot.pitch;
        float yaw = pivot.yaw;
        float roll = pivot.roll;
        Roll.text = roll.ToString("F4");
        Pitch.text = pitch.ToString("F4");
        Yaw.text = yaw.ToString("F4");

        // Linear
        float vv = pivot.vel.y;
        float va = pivot.accel.y;
        VV.text = vv.ToString("F4");
        VA.text = va.ToString("F4");

        // Angular Velocity
        float avroll = pivot.aVel.z;
        float avpitch = pivot.aVel.x;
        float avyaw = pivot.aVel.y;
        AVRoll.text = avroll.ToString("F4");
        AVPitch.text = avpitch.ToString("F4");
        AVYaw.text = avyaw.ToString("F4");

        // Angular Acceleration
        float aaroll = pivot.aAccel.z;
        float aapitch = pivot.aAccel.x;
        float aayaw = pivot.aAccel.y;
        AARoll.text = aaroll.ToString("F4");
        AAPitch.text = aapitch.ToString("F4");
        AAYaw.text = aayaw.ToString("F4");

        // Position
        PosX.text = pivot.pos.x.ToString("F4");
        PosY.text = pivot.pos.z.ToString("F4");
        float alt = pivot.pos.y - Pivot.FRAMEHEIGHT;
        ALT.text = alt.ToString("F4");

        // Groundspeed
        Vector3 hzntlVelo = new Vector3(pivot.vel.x, 0, pivot.vel.z);
        float speed = (float) Math.Sqrt(hzntlVelo.sqrMagnitude);
        float heading = Quaternion.LookRotation(hzntlVelo).eulerAngles.y;
        if (speed < 0.001f)
        {
            heading = 0f;
        }
        GRSP.text = speed.ToString("F4");
        HEAD.text = heading.ToString("F4");

    }

    void FixedUpdate()
    {
        updateDashboard(body);
    }
}
