using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelDriftTrail : MonoBehaviour
{
    public float dotThreshold = 0.5f;
    [HideInInspector]
    public bool shouldEmit = false;

    private TrailRenderer trailRenderer;
    private Wheel wheel;

    private void Awake()
    {
        wheel = GetComponentInParent<Wheel>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        Vector3 wheelVelocityDir = wheel.carRigidbody.GetPointVelocity(wheel.transform.position).normalized;
        Vector3 wheelDir = wheel.transform.forward;
        float dot = Vector3.Dot(wheelVelocityDir, wheelDir);
        trailRenderer.emitting = (dot < dotThreshold && wheel.IsOnGround) || shouldEmit;
    }
}
