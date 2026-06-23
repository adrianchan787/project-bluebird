using UnityEngine;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class Config : MonoBehaviour
{

    [SerializeField] private GameObject menuPanel; 
    [SerializeField] private ScaleScript scalescript;

    // COM calculations
    public float SCALE = 0.1f;
    public float LOCALCENTROID;
    public float WEIGHT = 0.06f;
    public float LINEAR_DRAG = 1.0f;
    public float ANGULAR_DRAG = 2.8f;
    public float PROP_DRAG_COEFF = 0.6f;
    public float MAX_PROP_THRUST = 0.35f;
    public Vector3 com;

    [System.Serializable]
    public class PID
    {
        [SerializeField] private TMP_InputField p_field;
        [SerializeField] private TMP_InputField i_field;
        [SerializeField] private TMP_InputField d_field;
        [SerializeField] private TMP_InputField ic_field;

        [HideInInspector] public float proportional;
        [HideInInspector] public float integral;
        [HideInInspector] public float derivative;
        [HideInInspector] public float integral_clamp;

        public PID(float p, float i, float d, float i_clamp)
        {
            proportional = p;
            integral = i;
            derivative = d;
            integral_clamp = i_clamp;
        }

        public void updateField()
        {
            readField(p_field, val => proportional = val);
            readField(i_field, val => integral = val);
            readField(d_field, val => derivative = val);
            readField(ic_field, val => integral_clamp = val);
        }

        public void writeField()
        {
            p_field.text = proportional.ToString("F2");
            i_field.text = integral.ToString("F2");
            d_field.text = derivative.ToString("F2");
            ic_field.text = integral_clamp.ToString("F2");
        }
        
    }

    [SerializeField] private TMP_InputField weight, max_thrust, scale, COMX, COMY, COMZ, lin_drag, ang_drag, prop_drag;

    public PID VV = new PID(20.0f, 1.0f, -2.0f, 100.0f);
    public PID HV = new PID(40.0f, 4.0f, 0.0f, 25.0f);
    public PID P = new PID(0.2f, 0.0f, 0.0f, 100.0f);
    public PID R = new PID(0.4f, 0.0f, 0.0f, 100.0f);
    public PID Y = new PID(0.02f, 0.0f, 0.0f, 100.0f);
    public PID YV = new PID(10.0f, 0.3f, 0.01f, 100.0f);
    public PID PV = new PID(10.1f, 0.03f, 0.0f, 0.0f);
    public PID RV = new PID(1.1f, 0.5f, 0.0f, 100.0f);

    private static void readField(TMP_InputField field, Action<float> assign)
    {
        field.onEndEdit.AddListener(text => {
            if (float.TryParse(text, out float result))
            {
                assign(result);
            }
        });
    }

    private void InitializeListeners()
    {
        readField(weight, val => WEIGHT = val);
        readField(lin_drag, val => LINEAR_DRAG = val);
        readField(ang_drag, val => ANGULAR_DRAG = val);
        readField(prop_drag, val => PROP_DRAG_COEFF = val);
        readField(max_thrust, val => MAX_PROP_THRUST = val);
        readField(scale, val => {SCALE = val; scalescript.ApplyScale(val);});

        readField(COMX, val => com = new Vector3(val, com.y, com.z));
        readField(COMY, val => com = new Vector3(com.x, val, com.z));
        readField(COMZ, val => com = new Vector3(com.x, com.y, val));

        VV.updateField();
        HV.updateField();
        P.updateField();
        R.updateField();
        Y.updateField();
        YV.updateField();
        PV.updateField();
        RV.updateField();
    }

    void Start()
    {
        LOCALCENTROID = (float)(-0.8f / Math.Sqrt(3) * SCALE);
        com = new Vector3(0, (float) 0, LOCALCENTROID);
        InitializeListeners();
    }

    void Update()
    {
        bool isActive = menuPanel.activeSelf;
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            menuPanel.SetActive(!isActive);
            if (!isActive)
            {
                Time.timeScale = 0f;
                weight.text = WEIGHT.ToString("F4");
                lin_drag.text = LINEAR_DRAG.ToString("F2");
                prop_drag.text = PROP_DRAG_COEFF.ToString("F2");
                ang_drag.text = ANGULAR_DRAG.ToString("F2");
                max_thrust.text = MAX_PROP_THRUST.ToString("F4");
                scale.text = SCALE.ToString("F2");
                COMX.text = com.x.ToString("F4");
                COMY.text = com.y.ToString("F4");
                COMZ.text = com.z.ToString("F4");
                VV.writeField();
                HV.writeField();
                P.writeField();
                R.writeField();
                Y.writeField();
                YV.writeField();
                PV.writeField();
                RV.writeField();
            } else
            {
                Time.timeScale = 1f;
            }
        }
    }
}
