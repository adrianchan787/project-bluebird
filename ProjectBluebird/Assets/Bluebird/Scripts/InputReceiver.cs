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
    public InputActionReference q_p;
    public InputActionReference q_n;
    public InputActionReference w_p;
    public InputActionReference w_n;
    public InputActionReference e_p;
    public InputActionReference e_n;
    public InputActionReference r_p;
    public InputActionReference r_n;

    public InputActionReference t_p;
    public InputActionReference t_n;

    void Start()
    {
        OnDisable();
    }

    void OnEnable()
    {
        SubscribeAction(t_p, OnVvP);
        SubscribeAction(t_n, OnVvN);
        SubscribeAction(q_p, OnPitchP);
        SubscribeAction(q_n, OnPitchN);
        SubscribeAction(e_p, OnRollP);
        SubscribeAction(e_n, OnRollN);
        SubscribeAction(w_p, OnYawP);
        SubscribeAction(w_n, OnYawN);
        SubscribeAction(r_p, OnHvP);
        SubscribeAction(r_n, OnHvN);     
    }

    void OnDisable()
    {
        UnsubscribeAction(t_p, OnVvP);
        UnsubscribeAction(t_n, OnVvN);
        UnsubscribeAction(q_p, OnPitchP);
        UnsubscribeAction(q_n, OnPitchN);
        UnsubscribeAction(e_p, OnRollP);
        UnsubscribeAction(e_n, OnRollN);
        UnsubscribeAction(w_p, OnYawP);
        UnsubscribeAction(w_n, OnYawN);
        UnsubscribeAction(r_p, OnHvP);
        UnsubscribeAction(r_n, OnHvN); 
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
                expected_hztlv = 0.0f;
            }
        }
    }
}