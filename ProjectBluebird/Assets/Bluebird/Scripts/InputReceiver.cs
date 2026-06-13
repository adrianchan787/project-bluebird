using UnityEngine;
using UnityEngine.InputSystem;

public class InputReceiver : MonoBehaviour
{
    [Header("Flight Targets")]
    public float expected_vv = 0.0f;
    public float expected_pitch = 0.0f;
    public float expected_roll = 0.0f;
    public float expected_yaw = 0.0f;
    public float expected_hztlv = 0f;

    [Header("Tuning Speeds (Units per Click)")]
    [SerializeField] private float vv_speed = 0.1f;

    [SerializeField] private float hv_speed = 0.25f;
    [SerializeField] private float angle_speed = 1f;

    [Header("Controls")]
    public InputActionReference vv_p;
    public InputActionReference vv_n;
    public InputActionReference pitch_p;
    public InputActionReference pitch_n;
    public InputActionReference roll_p;
    public InputActionReference roll_n;
    public InputActionReference yaw_p;
    public InputActionReference yaw_n;

    public InputActionReference hv_p;
    public InputActionReference hv_n;

    void Start()
    {
        OnDisable();
    }

    void OnEnable()
    {
        SubscribeAction(vv_p, OnVvP);
        SubscribeAction(vv_n, OnVvN);
        SubscribeAction(pitch_p, OnPitchP);
        SubscribeAction(pitch_n, OnPitchN);
        SubscribeAction(roll_p, OnRollP);
        SubscribeAction(roll_n, OnRollN);
        SubscribeAction(yaw_p, OnYawP);
        SubscribeAction(yaw_n, OnYawN);
        SubscribeAction(hv_p, OnHvP);
        SubscribeAction(hv_n, OnHvN);     
    }

    void OnDisable()
    {
        UnsubscribeAction(vv_p, OnVvP);
        UnsubscribeAction(vv_n, OnVvN);
        UnsubscribeAction(pitch_p, OnPitchP);
        UnsubscribeAction(pitch_n, OnPitchN);
        UnsubscribeAction(roll_p, OnRollP);
        UnsubscribeAction(roll_n, OnRollN);
        UnsubscribeAction(yaw_p, OnYawP);
        UnsubscribeAction(yaw_n, OnYawN);
        UnsubscribeAction(hv_p, OnHvP);
        UnsubscribeAction(hv_n, OnHvN);   
    }

    private void OnVvP(InputAction.CallbackContext ctx) => expected_vv += vv_speed;
    private void OnVvN(InputAction.CallbackContext ctx) => expected_vv -= vv_speed;

    private void OnPitchP(InputAction.CallbackContext ctx) => expected_pitch += angle_speed;
    private void OnPitchN(InputAction.CallbackContext ctx) => expected_pitch -= angle_speed;

    private void OnRollP(InputAction.CallbackContext ctx) => expected_roll += angle_speed;
    private void OnRollN(InputAction.CallbackContext ctx) => expected_roll -= angle_speed;

    private void OnYawP(InputAction.CallbackContext ctx) => expected_yaw += angle_speed;
    private void OnYawN(InputAction.CallbackContext ctx) => expected_yaw -= angle_speed;
    private void OnHvP(InputAction.CallbackContext ctx) => expected_hztlv += hv_speed;
    private void OnHvN(InputAction.CallbackContext ctx) => expected_hztlv -= hv_speed;

    private void SubscribeAction(InputActionReference actionRef, System.Action<InputAction.CallbackContext> callback)
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.Enable();
            actionRef.action.performed += callback;
        }
    }

    private void UnsubscribeAction(InputActionReference actionRef, System.Action<InputAction.CallbackContext> callback)
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.performed -= callback;
            actionRef.action.Disable();
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            FlightController.on = !FlightController.on;
            if (FlightController.on)
            {
                OnEnable();
            } else
            {
                OnDisable();
                expected_vv = 0.0f;
                expected_pitch = 0.0f;
                expected_roll = 0.0f;
                expected_yaw = 0.0f;
                expected_hztlv = 0.0f;
            }
        }
    }
}