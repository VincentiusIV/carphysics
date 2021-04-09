using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Rigidbody carRigidbody;
    public bool IsOnGround { get; private set; }
    public float radius;
    public float staticFriction;
    public float kineticFriction;
    public float enginePower, brakePower, steerPower;
    public float drag;
    public float mass;
    public float steerSpeed = 5;
    public float steerAngle = 25;
    public bool canSteer;
    [Tooltip("Rolling resistance should be about 30x drag")]
    public float rollingResistance;

    public float F_long { get; private set;}
    public float F_lat { get; private set;}

    private float steer, gasPedal;

    public void Reset()
    {
        radius = .5f;
        mass = 15;
        staticFriction = 1;
        kineticFriction = 0.7f;
        
        enginePower = 50;
        brakePower = 10;
        drag = 0.05f;
        rollingResistance = 1.5f;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void Awake()
    {

    }

    public void Accelerate(float gasPedal)
    {
        this.gasPedal = gasPedal;
    }

    public void Steer(float steer)
    {
        this.steer = steer;
    }

    private void FixedUpdate()
    {
        if(canSteer)
        {
            float yRot = transform.localEulerAngles.y;
            float targetYRot = steer * steerAngle;
            Quaternion targetRot = Quaternion.Euler(0, targetYRot, 0);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, steerSpeed * Time.fixedDeltaTime);
        }

        // Only apply traction force if the wheel is touching smth on the ground.
        RaycastHit hit;
        IsOnGround = Physics.Raycast(transform.position, Vector3.down, out hit, radius);
        if(IsOnGround)
            IsOnGround &= (hit.collider.GetComponent<Car>() == null); // ignore hits on car.
        if (!IsOnGround)
        {
            return;
        }
        Debug.DrawLine(transform.position, hit.point);

        Vector3 velocity = carRigidbody.GetPointVelocity(transform.position);

        F_long = 0;
        if (gasPedal > 0)
        {
            float f_gas = gasPedal * enginePower * radius;
            if (f_gas <= staticFriction * Physics.gravity.magnitude * mass)
            {
                F_long = f_gas;
            }
            else
            {
                Debug.Log("Wheelspin");
                F_long = kineticFriction * 9.81f * mass;
            }
        }
        else
        {
            F_long = gasPedal * brakePower * Mathf.Sign(velocity.z);
        }

        float xVel = transform.InverseTransformDirection(velocity).x;
        F_lat = kineticFriction * 9.81f * mass * -Mathf.Sign(xVel);
        // Friction force is constant, we need to ensure the velocity change during the next frame doesn't exceed the velocity causing the friction
        F_lat = Mathf.Clamp(F_lat, -Mathf.Abs(xVel) * mass / Time.fixedDeltaTime, Mathf.Abs(xVel) * mass / Time.fixedDeltaTime);
        Vector3 F_traction = transform.forward * F_long + transform.right * F_lat;

        Vector3 F_drag = -drag * velocity.normalized * velocity.sqrMagnitude;
        F_traction += F_drag;

        carRigidbody.AddForceAtPosition(F_traction, transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * radius);
    }
}
