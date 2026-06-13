using UnityEngine;
using System;

public class FlightController : MonoBehaviour
{
    // State struct
    public struct State
    {
        public float L_speed { get; set; } 
        public float R_speed { get; set; }
        public float P_speed { get; set; }
        public float T_speed { get; set; }
        public float S_deg { get; set; }

        public State(float l, float r, float b, float t, float s_deg)
        {
            L_speed = l;
            R_speed = r;
            P_speed = b;
            T_speed = t;
            S_deg = s_deg;
        }
    }

    private State state;

    [SerializeField] private InputReceiver ir;
    
    [SerializeField] private Pivot pivot;
    [SerializeField] private Config c;

    // Integrated Errors

    public static bool on = false;

    private float integratedVertVeloError = 0;
    private float integratedHztlVeloError = 0;
    private float integratedPitchError = 0;
    private float integratedRollError = 0;
    private float integratedYawError = 0;
    private float integratedVYawError = 0;

    private float integratedVPitchError = 0;

    private float integratedVRollError = 0;

    // Local copies of expected values
    private float expected_vv = 0;
    private float expected_hztlv = 0;
    private float expected_pitch = 0;
    private float expected_roll = 0;
    private float expected_yaw = 0;


    // Public Mathematical Functions
    public static float Clamp(float value, float min, float max) {
        return (value < min) ? min : (value > max) ? max : value;
    }

    public static float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }

    private void updateIntegratedAngleError(float receiver_val, float actual_val, ref float fc_val, ref float error, float clamp_error)
    {
        if (receiver_val != fc_val)
        {
            fc_val = receiver_val;
        }
        error += Time.fixedDeltaTime * Mathf.DeltaAngle(actual_val, fc_val);
        error = Clamp(error, -clamp_error, clamp_error);
    }

    void Start()
    {
        state = new State(0, 0, 0, 0, 0);
    }

    private float pid_gain(float p_gain, float e, float i_gain, float i, float d_gain, float d)
    {
        return p_gain * (e + i_gain * i + d_gain * d);
    }

    private void updateIntegratedError(float receiver_val, float actual_val, ref float fc_val, ref float error, float clamp_error)
    {
        if (receiver_val != fc_val)
        {
            fc_val = receiver_val;
        }
        error += Time.fixedDeltaTime * (fc_val - actual_val);
        error = Clamp(error, -clamp_error, clamp_error);
    }

    private void updateIntegratedInnerError(float receiver_val, float actual_val, ref float error, float clamp_error)
    {
        error += Time.fixedDeltaTime * (receiver_val - actual_val);
        error = Clamp(error, -clamp_error, clamp_error);
    }

    
    private State calculateStateGain(Pivot p)
    {
        State return_state = new State(0, 0, 0, 0, 0f);

        // Update for Vertical Velocity
        float vv_gain = pid_gain(c.VV.proportional, (expected_vv - p.vel.y), c.VV.integral, integratedVertVeloError, c.VV.derivative, p.accel.y);
        return_state.L_speed += vv_gain;
        return_state.R_speed += vv_gain;
        return_state.P_speed += vv_gain;
        
        // Update for Pitch
        float target_vpitch = pid_gain(c.P.proportional, (float) Mathf.DeltaAngle(p.pitch, expected_pitch), c.P.integral, integratedPitchError, c.P.derivative, p.aVel.x);
        updateIntegratedInnerError(target_vpitch, pivot.aVel.x, ref integratedVPitchError, c.PV.integral_clamp);
        float pitch_gain = pid_gain(c.PV.proportional, (target_vpitch - p.aVel.x), c.PV.integral, integratedVPitchError, c.PV.derivative, p.aAccel.x);
        return_state.L_speed += pitch_gain;
        return_state.R_speed += pitch_gain;
        return_state.P_speed -= pitch_gain;

        // Update for Roll
        float target_vroll = pid_gain(c.R.proportional, (float) Mathf.DeltaAngle(p.roll, expected_roll), c.R.integral, integratedRollError, c.R.derivative, p.aVel.z);
        updateIntegratedInnerError(target_vroll, pivot.aVel.z, ref integratedVRollError, c.RV.integral_clamp);
        float roll_gain = pid_gain(c.RV.proportional, (target_vroll - p.aVel.z), c.RV.integral, integratedVRollError, c.RV.derivative, p.aAccel.z);
        return_state.L_speed -= roll_gain;
        return_state.R_speed += roll_gain;

        // Update for Yaw
        // This is the target yaw velocity needed
        float target_vyaw = pid_gain(c.Y.proportional, (float) Mathf.DeltaAngle(p.yaw, expected_yaw), c.Y.integral, integratedYawError, c.Y.derivative, p.aVel.y);
        updateIntegratedInnerError(target_vyaw, pivot.aVel.y, ref integratedVYawError, c.YV.integral_clamp);
        float vyaw_gain = pid_gain(c.YV.proportional, (target_vyaw - p.aVel.y), c.YV.integral, integratedVYawError, c.YV.derivative, p.aAccel.y);
        return_state.S_deg = vyaw_gain;
        

        // Update for Horizontal Velocity
        Vector3 hzntlAccel = new Vector3(p.accel.x, 0, p.accel.z);
        float hzntl_accel = (float) Math.Sqrt(hzntlAccel.sqrMagnitude);
        float hv_gain = pid_gain(c.HV.proportional, (expected_hztlv - p.hzntl_speed), c.HV.integral, integratedHztlVeloError, c.HV.derivative, hzntl_accel);
        return_state.T_speed += hv_gain;
        
        return return_state;
    }
    


    void FixedUpdate()
    {
        if (on == false) 
        {
            integratedHztlVeloError = 0f;
            integratedYawError = 0f;
            integratedPitchError = 0f;
            integratedRollError = 0f;
            integratedVYawError = 0f;
            integratedVPitchError = 0f;
            integratedVRollError = 0f;
            expected_vv = ir.expected_vv;
            expected_hztlv = ir.expected_hztlv;
            expected_pitch = ir.expected_pitch;
            expected_roll = ir.expected_roll;
            expected_yaw = ir.expected_yaw;
            pivot.leftRotor.speed = 0f;
            pivot.rightRotor.speed = 0f;
            pivot.pitchRotor.speed = 0f;
            pivot.thrustRotor.speed = 0f;
            state = new State(0, 0, 0, 0, 0);
        } else
        {
            updateIntegratedError(ir.expected_vv, pivot.vel.y, ref expected_vv, ref integratedVertVeloError, c.VV.integral_clamp);
            updateIntegratedError(ir.expected_hztlv, pivot.hzntl_speed, ref expected_hztlv, ref integratedHztlVeloError, c.HV.integral_clamp);
            updateIntegratedAngleError(ir.expected_pitch, pivot.pitch, ref expected_pitch, ref integratedPitchError, c.P.integral_clamp);
            updateIntegratedAngleError(ir.expected_roll, pivot.roll, ref expected_roll, ref integratedRollError, c.R.integral_clamp);
            updateIntegratedAngleError(ir.expected_yaw, pivot.yaw, ref expected_yaw, ref integratedYawError, c.Y.integral_clamp);
            state = calculateStateGain(pivot);
            state.L_speed = Clamp(state.L_speed, 0, Rotor.MAXDISPLAYSPEED); 
            state.R_speed = Clamp(state.R_speed, 0, Rotor.MAXDISPLAYSPEED);
            state.P_speed = Clamp(state.P_speed, 0, Rotor.MAXDISPLAYSPEED);
            state.T_speed = Clamp(state.T_speed, 0, Rotor.MAXDISPLAYSPEED);
            state.S_deg = Clamp(state.S_deg, -20, 20);
            pivot.leftRotor.speed = state.L_speed;
            pivot.rightRotor.speed = state.R_speed;
            pivot.pitchRotor.speed = state.P_speed;
            pivot.thrustRotor.speed = state.T_speed;
            pivot.pitchMotor.direction = state.S_deg;
        }
    }
}
