using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool IsOnGround { get; private set; }
    public float F_long { get; private set; }
    public float F_lat { get; private set; }
    public Rigidbody carRigidbody;    
    public float radius;
    public Transform wheelMesh;

    public float brakePower;
    public float mass;

    [Header("Tyre")]
    public float staticFriction;
    public float kineticFriction;
    public float rollingResistance;

    [Header("Steering")]
    public bool canSteer;
    public float steerAngle = 25;
    public float steerSpeed = 5;

    [Header("Suspension")]
    public float suspensionLength = 0;
    public float springStiffness, damperStiffness;

    private Vector3 localWheelJointPosition, wheelJointPosition; 
    private Vector3 springVelocity;
    private Vector3 lastWheelPosition;
    private Quaternion currentVerticalRotation;
    private Rigidbody wheelRigidbody;
    private float steer;
    private float engineTorque;
    private float brakePedal;

    public void Reset()
    {
        radius = .5f;
        mass = 15;
        staticFriction = 1;
        kineticFriction = 0.7f;
        brakePower = 10;

        rollingResistance = 1.5f;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void Awake()
    {
        wheelRigidbody = GetComponent<Rigidbody>();
        localWheelJointPosition = carRigidbody.transform.InverseTransformPoint(transform.position);
    }

    public void Accelerate(float engineTorque)
    {
        this.engineTorque = engineTorque;
    }

    public void Brake(float brakePedal)
    {
        this.brakePedal = brakePedal;
    }

    public void Steer(float steer)
    {
        this.steer = steer;
    }

    private void FixedUpdate()
    {
        UpdateSteering();
        ApplySuspensionForce();
        UpdateWheelMeshRotation();
    }

    private void OnCollisionStay(Collision collision)
    {
        ApplyTractionForce(collision);
    }

    private void UpdateSteering()
    {
        if (canSteer)
        {
            float targetYRot = steer * steerAngle;
            currentVerticalRotation = Quaternion.Lerp(currentVerticalRotation, Quaternion.Euler(0, targetYRot, 0), steerSpeed * Time.fixedDeltaTime);
            Quaternion targetRot = carRigidbody.rotation * currentVerticalRotation;
            wheelRigidbody.MoveRotation(targetRot);
        }
        else
        {
            wheelRigidbody.MoveRotation(carRigidbody.rotation);
        }
    }

    private void ApplyTractionForce(Collision collision)
    {
        Vector3 velocity = carRigidbody.GetPointVelocity(transform.position);

        F_long = 0;
        if (brakePedal < 0)
        {
            F_long = brakePedal * brakePower;
        }
        else if (engineTorque > 0)
        {
            float f_gas = engineTorque / radius;
            if (f_gas <= staticFriction * Physics.gravity.magnitude * mass)
            {
                F_long = f_gas;
            }
            else
            {
                Debug.Log("Wheelspin");
                F_long = kineticFriction * Physics.gravity.magnitude * mass;
            }
        }

        float xVel = transform.InverseTransformDirection(velocity).x;
        F_lat = kineticFriction * Physics.gravity.magnitude * mass * -Mathf.Sign(xVel);
        // Friction force is constant, we need to ensure the velocity change during the next frame doesn't exceed the velocity causing the friction
        F_lat = Mathf.Clamp(F_lat, -Mathf.Abs(xVel) * mass / Time.fixedDeltaTime, Mathf.Abs(xVel) * mass / Time.fixedDeltaTime);
        Vector3 F_traction = transform.forward * F_long + transform.right * F_lat;
        
        Vector3 F_rr = -rollingResistance * velocity;
        F_traction += F_rr;

        carRigidbody.AddForceAtPosition(F_traction, wheelRigidbody.position - transform.up * radius); // apply traction force at contact point with ground.
    }

    private void ApplySuspensionForce()
    {
        wheelJointPosition = localWheelJointPosition - Vector3.up * suspensionLength;

        Vector3 velocityAtJoint = carRigidbody.GetRelativePointVelocity(wheelJointPosition);
        Vector3 wheelVelocity = wheelRigidbody.velocity;
        Vector3 jointPosition = carRigidbody.transform.TransformPoint(wheelJointPosition);        
        Vector3 F_spring = -springStiffness * (jointPosition - wheelRigidbody.position); // Spring-damper system F = -kx - bv
        Vector3 F_suspension = F_spring - damperStiffness * (springVelocity / Time.fixedDeltaTime);        
        carRigidbody.AddForceAtPosition(F_suspension, wheelRigidbody.position);

        wheelRigidbody.AddForce(-F_suspension / carRigidbody.mass);

        springVelocity = wheelRigidbody.position - jointPosition;

        // Constrain wheel rigidbody position (xdist=0, ydist=suspensionLength, zdist=0).        
        Vector3 wheelPosition = wheelJointPosition;
        wheelPosition.y = (carRigidbody.transform.InverseTransformPoint(wheelRigidbody.position)).y;
        wheelPosition.y = Mathf.Clamp(wheelPosition.y, wheelJointPosition.y - suspensionLength, wheelJointPosition.y + suspensionLength);
        Vector3 worldPos = carRigidbody.position + carRigidbody.rotation * wheelPosition;

        wheelRigidbody.MovePosition(worldPos);
    }

    private void UpdateWheelMeshRotation()
    {
        Vector3 currentPos = wheelRigidbody.position;
        Vector3 delta = (currentPos - lastWheelPosition);
        float angleRad = delta.magnitude / radius;
        float angle = Mathf.Rad2Deg * angleRad;
        float sign = Vector3.Dot(carRigidbody.transform.forward, delta.normalized);
        Quaternion rot = Quaternion.Euler(sign * angle, 0, 0);
        wheelMesh.rotation *= rot;
        lastWheelPosition = currentPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * radius);
        
        Gizmos.color = Color.cyan;
        if(!Application.isPlaying)
        {
            wheelRigidbody = GetComponent<Rigidbody>();
            localWheelJointPosition = carRigidbody.transform.InverseTransformPoint(transform.position);
        }
        Vector3 worldWheelJointPosition = carRigidbody.transform.TransformPoint(wheelJointPosition);
        Gizmos.DrawLine(transform.position, worldWheelJointPosition);
        Gizmos.DrawWireSphere(worldWheelJointPosition, 0.1f);

    }

    internal void ClearVelocities()
    {
        wheelRigidbody.velocity = Vector3.zero;
        wheelRigidbody.angularVelocity = Vector3.zero;
        wheelJointPosition = localWheelJointPosition - Vector3.up * suspensionLength;
        wheelRigidbody.transform.position = carRigidbody.transform.TransformPoint(wheelJointPosition);
        springVelocity = Vector3.zero;
        lastWheelPosition = wheelRigidbody.transform.position;
        engineTorque = brakePedal = 0;
    }
}