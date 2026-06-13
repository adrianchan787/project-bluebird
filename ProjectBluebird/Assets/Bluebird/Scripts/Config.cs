using UnityEngine;
using System;

public class Config : MonoBehaviour
{

    // COM calculations
    private static readonly float LOCALCENTROID = (float)(-0.08f / Math.Sqrt(3));
    public float WEIGHT = 0.06f;
    public float LINEAR_DRAG = 1.0f;
    public float ANGULAR_DRAG = 2.8f;
    public float PROP_DRAG_COEFF = 0.6f;
    public float MAX_PROP_THRUST = 0.35f;
    public Vector3 com = new Vector3(0, (float) 0, LOCALCENTROID);

    [System.Serializable]
    public struct PID
    {
        public float proportional;
        public float integral;
        public float derivative;
        public float integral_clamp;

        public PID(float p, float i, float d, float i_clamp)
        {
            proportional = p;
            integral = i;
            derivative = d;
            integral_clamp = i_clamp;
        }
    }

    public PID VV = new PID(20.0f, 1.0f, -2.0f, 100.0f);
    public PID HV = new PID(40.0f, 4.0f, 0.0f, 25.0f);
    public PID P = new PID(0.2f, 0.0f, 0.0f, 100.0f);
    public PID R = new PID(0.4f, 0.0f, 0.0f, 100.0f);
    public PID Y = new PID(0.02f, 0.0f, 0.0f, 100.0f);
    public PID YV = new PID(10.0f, 0.3f, 0.01f, 100.0f);
    public PID PV = new PID(10.1f, 0.03f, 0.0f, 0.0f);
    public PID RV = new PID(1.1f, 0.5f, 0.0f, 100.0f);

    /*
        public float PGAIN_VV = 20.0f;
        public float IGAIN_VV = 1.0f;
        public float DGAIN_VV = -2.0f;
        public float ICLAMP_VV = 100.0f;

        public float PGAIN_HV = 40.0f;
        public float IGAIN_HV = 4.0f;
        public float DGAIN_HV = 0.0f;
        public float ICLAMP_HV = 25.0f;

        public float PGAIN_P = 0.2f;
        public float IGAIN_P = 0.0f;
        public float DGAIN_P = 0.0f;
        public float ICLAMP_P = 100.0f;

        public float PGAIN_R = 0.4f;
        public float IGAIN_R = 0.0f;
        public float DGAIN_R = 0.0f;
        public float ICLAMP_R = 8.0f;

        public float PGAIN_Y = 0.02f;
        public float IGAIN_Y = 0.0f;
        public float DGAIN_Y = 0.0f;
        public float ICLAMP_Y = 100.0f;

        public float PGAIN_YV = 10.0f;
        public float IGAIN_YV = 0.3f;
        public float DGAIN_YV = 0.01f;
        public float ICLAMP_YV = 100.0f;

        public float PGAIN_PV = 10.1f;
        public float IGAIN_PV = 0.03f;
        public float DGAIN_PV = 0.0f;
        public float ICLAMP_PV = 0.0f;

        public float PGAIN_RV = 1.1f;
        public float IGAIN_RV = 0.5f;
        public float DGAIN_RV = 0.0f;
        public float ICLAMP_RV = 100.0f;
    */
}
