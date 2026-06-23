using UnityEngine;
using Cinemachine;
using TMPro;
using System;

public class ScaleScript : MonoBehaviour
{
    private static float BASESCALE = 0.1f;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Cinemachine3rdPersonFollow follow;
    private float originalDistance;
    private Vector3 originalShoulderOffset;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private TMP_InputField COMX, COMY, COMZ;
    [SerializeField] private Pivot pivot;

    [SerializeField] private Config c;
    void Start()
    {
        follow = cam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        originalDistance = follow.CameraDistance;
        originalShoulderOffset = follow.ShoulderOffset;
    }

    public void ApplyScale(float newScale)
    {
        pivot.transform.localScale = new Vector3(newScale, newScale, newScale);

        c.LOCALCENTROID = (float)(-0.8f / Math.Sqrt(3) * newScale);
        c.com = new Vector3(0, 0, c.LOCALCENTROID);
        
        if (rb != null)
        {
            rb.centerOfMass = c.com;
            rb.ResetInertiaTensor(); 
        }

        COMX.text = c.com.x.ToString("F4");
        COMY.text = c.com.y.ToString("F4");
        COMZ.text = c.com.z.ToString("F4");

        follow.CameraDistance = originalDistance * (newScale / BASESCALE) * 1.1f;
        follow.ShoulderOffset.y = originalShoulderOffset.y + (newScale / BASESCALE) * 0.07f;
    }
}
