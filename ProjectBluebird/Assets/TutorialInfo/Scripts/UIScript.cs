using UnityEngine;
using TMPro;

public class UIScript : MonoBehaviour {
    public Pivot body;

    [SerializeField] private TMP_InputField LT, RT, PT, TT, VV, VA, Yaw, Pitch, Roll, AVYaw, AVPitch, AVRoll, AAYaw, AAPitch, AARoll, PosX, PosY, ALT, GRSP, HEAD;

    private static Vector3 calculateOrientation(Quaternion q) {
        float pitch = Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z);
        float yaw = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
        float roll = Mathf.Rad2Deg * Mathf.Asin(2 * q.x * q.y + 2 * q.z * q.w);

        return new Vector3(roll, yaw, pitch);
    }


    private void updateDashboard(Pivot pivot) {

        // Thrust Levels
        float LTval = 100f * pivot.leftRotor.thrustRatio;
        float RTval = 100f * pivot.rightRotor.thrustRatio;
        float PTval = 100f * pivot.pitchRotor.thrustRatio;
        float TTval = 100f * pivot.thrustRotor.thrustRatio;
        LT.text = LTval.ToString("F4");
        RT.text = RTval.ToString("F4");
        PT.text = PTval.ToString("F4");
        TT.text = TTval.ToString("F4");

        // Orientation
        Vector3 orientation = calculateOrientation(pivot.orientation);
        float roll = orientation.x;
        float pitch = orientation.z;
        float yaw = orientation.y;
        Roll.text = roll.ToString("F4");
        Pitch.text = pitch.ToString("F4");
        Yaw.text = yaw.ToString("F4");

        // Linear
        float vv = pivot.vel.y;
        float va = pivot.accel.y;
        VV.text = vv.ToString("F4");
        VA.text = va.ToString("F4");

        // Angular Velocity
        float avroll = pivot.aVel.x;
        float avpitch = pivot.aVel.z;
        float avyaw = pivot.aVel.y;
        AVRoll.text = avroll.ToString("F4");
        AVPitch.text = avpitch.ToString("F4");
        AVYaw.text = avyaw.ToString("F4");

        // Angular Acceleration
        float aaroll = pivot.aAccel.x;
        float aapitch = pivot.aAccel.z;
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
        float speed = hzntlVelo.sqrMagnitude;
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
