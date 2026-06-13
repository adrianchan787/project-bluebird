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

        public static State operator +(State current, State other)
        {
            return new State(current.L_speed + other.L_speed, current.R_speed + other.R_speed, current.P_speed + other.P_speed, current.T_speed + other.T_speed, current.S_deg + other.S_deg);
        }
    }

    private State state;

    public InputReceiver ir;
    
    public Pivot pivot;

    // Integrated Errors

    public float debug_p = 0;

    public float debug_i = 0;

    public float debug_d = 0;
    public float debug_total = 0;
    public float debug_targetv = 0;
    private float integratedVertVeloError = 0;
    private float integratedHztlVeloError = 0;
    public float integratedPitchError = 0;
    private float integratedRollError = 0;
    private float integratedYawError = 0;
    private float integratedVYawError = 0;

    public float integratedVPitchError = 0;

    private float integratedVRollError = 0;

    // Local copies of expected values
    private float expected_vv = 0;
    private float expected_hztlv = 0;
    private float expected_pitch = 0;
    private float expected_roll = 0;
    private float expected_yaw = 0;
    

    // PID Gains
    public const float PGAIN_VV = 20.0f;
    public const float IGAIN_VV = 1.0f;
    public const float DGAIN_VV = -2.0f;
    public const float ICLAMP_VV = 100.0f;

    public const float PGAIN_HV = 15.0f;
    public const float IGAIN_HV = 0.2f;
    public const float DGAIN_HV = -1.0f;
    public const float ICLAMP_HV = 10.0f;

    public const float PGAIN_P = 0.2f;
    public const float IGAIN_P = 0.0f;
    public const float DGAIN_P = 0.0f;
    public const float ICLAMP_P = 100.0f;

    public const float PGAIN_R = 0.4f;
    public const float IGAIN_R = 0.0f;
    public const float DGAIN_R = 0.0f;
    public const float ICLAMP_R = 8.0f;

    public const float PGAIN_Y = 0.02f;
    public const float IGAIN_Y = 0.0f;
    public const float DGAIN_Y = 0.0f;
    public const float ICLAMP_Y = 100.0f;

    public const float PGAIN_YV = 10.0f;
    public const float IGAIN_YV = 0.3f;
    public const float DGAIN_YV = 0.01f;
    public const float ICLAMP_YV = 100.0f;

    public const float PGAIN_PV = 10.1f;
    public const float IGAIN_PV = 0.00f;
    public const float DGAIN_PV = 0.0f;
    public const float ICLAMP_PV = 0.0f;
    public const float PGAIN_RV = 1.1f;
    public const float IGAIN_RV = 0.00f;
    public const float DGAIN_RV = 0.0f;
    public const float ICLAMP_RV = 0.0f;


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

    void Start()
    {
        state = new State(0, 0, 0, 0, 0);
    }

    private float pid_gain(float p_gain, float e, float i_gain, float i, float d_gain, float d)
    {
        if (p_gain == PGAIN_RV)
        {
            debug_total = p_gain * (e + i_gain * i + d_gain * d);
            debug_p = p_gain * e;
            debug_i = p_gain * i_gain * i;
            debug_d = p_gain * d_gain * d;
        }
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
        State return_state = new State(0, 0, 0, 0, 3.14f);

        // Update for Vertical Velocity
        float vv_gain = pid_gain(PGAIN_VV, (expected_vv - p.vel.y), IGAIN_VV, integratedVertVeloError, DGAIN_VV, p.accel.y);
        return_state.L_speed += vv_gain;
        return_state.R_speed += vv_gain;
        return_state.P_speed += vv_gain;
        
        // Update for Pitch
        float target_vpitch = pid_gain(PGAIN_P, (expected_pitch - p.pitch), IGAIN_P, integratedPitchError, DGAIN_P, p.aVel.x);
        debug_targetv = target_vpitch;
        updateIntegratedInnerError(target_vpitch, pivot.aVel.x, ref integratedVPitchError, ICLAMP_PV);
        float pitch_gain = pid_gain(PGAIN_PV, (target_vpitch - p.aVel.x), IGAIN_PV, integratedVPitchError, DGAIN_PV, p.aAccel.x);
        return_state.L_speed += pitch_gain;
        return_state.R_speed += pitch_gain;
        return_state.P_speed -= pitch_gain;

        // Update for Roll
        float target_vroll = pid_gain(PGAIN_R, (expected_roll - p.roll), IGAIN_R, integratedRollError, DGAIN_R, p.aVel.z);
        updateIntegratedInnerError(target_vroll, pivot.aVel.z, ref integratedVRollError, ICLAMP_RV);
        float roll_gain = pid_gain(PGAIN_RV, (target_vroll - p.aVel.z), IGAIN_RV, integratedVRollError, DGAIN_RV, p.aAccel.z);
        return_state.L_speed -= roll_gain;
        return_state.R_speed += roll_gain;

        // Update for Yaw
        // This is the target yaw velocity needed
        float target_vyaw = pid_gain(PGAIN_Y, (expected_yaw - p.yaw), IGAIN_Y, integratedYawError, DGAIN_Y, p.aVel.y);
        updateIntegratedInnerError(target_vyaw, pivot.aVel.y, ref integratedVYawError, ICLAMP_YV);
        float vyaw_gain = pid_gain(PGAIN_YV, (target_vyaw - p.aVel.y), IGAIN_YV, integratedVYawError, DGAIN_YV, p.aAccel.y);
        return_state.S_deg = vyaw_gain;
        

        // Update for Horizontal Velocity
        float hv_gain = pid_gain(PGAIN_HV, (expected_hztlv - p.vel.z), IGAIN_HV, integratedHztlVeloError, DGAIN_HV, p.accel.z);
        //return_state.T_speed += hv_gain;
        
        
        
        return return_state;
    }
    


    void FixedUpdate()
    {
        // Temporary stopgap
        if (pivot.pos.y <= 0.06f) 
        {
            integratedYawError = 0f;
            integratedPitchError = 0f;
            integratedRollError = 0f;
            integratedVYawError = 0f;
            integratedVPitchError = 0f;
            integratedVRollError = 0f;
        }
        updateIntegratedError(ir.expected_vv, pivot.vel.y, ref expected_vv, ref integratedVertVeloError, ICLAMP_VV);
        updateIntegratedError(ir.expected_hztlv, pivot.vel.z, ref expected_hztlv, ref integratedHztlVeloError, ICLAMP_HV);
        updateIntegratedError(ir.expected_pitch, pivot.pitch, ref expected_pitch, ref integratedPitchError, ICLAMP_P);
        updateIntegratedError(ir.expected_roll, pivot.roll, ref expected_roll, ref integratedRollError, ICLAMP_R);
        updateIntegratedError(ir.expected_yaw, pivot.yaw, ref expected_yaw, ref integratedYawError, ICLAMP_Y);
        state = calculateStateGain(pivot);
        state.L_speed = Clamp(state.L_speed, -Rotor.MAXDISPLAYSPEED, Rotor.MAXDISPLAYSPEED); 
        state.R_speed = Clamp(state.R_speed, -Rotor.MAXDISPLAYSPEED, Rotor.MAXDISPLAYSPEED);
        state.P_speed = Clamp(state.P_speed, -Rotor.MAXDISPLAYSPEED, Rotor.MAXDISPLAYSPEED);
        state.T_speed = Clamp(state.T_speed, -Rotor.MAXDISPLAYSPEED, Rotor.MAXDISPLAYSPEED);
        state.S_deg = Clamp(state.S_deg, -20, 20);
        pivot.leftRotor.speed = state.L_speed;
        pivot.rightRotor.speed = state.R_speed;
        pivot.pitchRotor.speed = state.P_speed;
        pivot.thrustRotor.speed = state.T_speed;
        pivot.pitchMotor.direction = state.S_deg;
    }
}
