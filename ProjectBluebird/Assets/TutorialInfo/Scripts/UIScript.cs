using UnityEngine;
using TMPro;

public class UIScript : MonoBehaviour {
    public Pivot body;

    [SerializeField] private TMP_InputField LT;
    [SerializeField] private TMP_InputField RT;
    [SerializeField] private TMP_InputField PT;
    [SerializeField] private TMP_InputField TT;
    [SerializeField] private TMP_InputField VV;
    [SerializeField] private TMP_InputField VA;
    [SerializeField] private TMP_InputField Yaw;
    [SerializeField] private TMP_InputField Pitch;
    [SerializeField] private TMP_InputField Roll; 
    [SerializeField] private TMP_InputField AVYaw; 
    [SerializeField] private TMP_InputField AVPitch; 
    [SerializeField] private TMP_InputField AVRoll; 
    [SerializeField] private TMP_InputField AAYaw; 
    [SerializeField] private TMP_InputField AAPitch; 
    [SerializeField] private TMP_InputField AARoll;


    public void updateDashboard(Pivot pivot) {
        float LTval = 100f * Pivot.thrustRatio(pivot.leftRotor);
        float RTval = 100f * Pivot.thrustRatio(pivot.rightRotor);
        float PTval = 100f * Pivot.thrustRatio(pivot.pitchRotor);
        float TTval = 100f * Pivot.thrustRatio(pivot.thrustRotor);
        LT.text = LTval.ToString("F4");
        RT.text = RTval.ToString("F4");
        PT.text = PTval.ToString("F4");
        TT.text = TTval.ToString("F4");
    }

    void FixedUpdate()
    {
        updateDashboard(body);
    }
}
