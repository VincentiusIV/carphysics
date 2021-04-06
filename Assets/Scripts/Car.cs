using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float enginePower = 50, brakePower = 25, steerPower = 10;
    public float drag = 0.05f, rollingResistance = 1.5f; // rolling resistance must be about 30x drag

    public Wheel[] wheels;
    private Rigidbody Rigidbody;
    private float steer, gasPedal;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Accelerate(float gasPedal)
    {
        this.gasPedal = gasPedal;
        foreach (var wheel in wheels)
        {
            wheel.Accelerate(gasPedal);
        }
    }

    public void Steer(float steer)
    {
        this.steer = steer;
        foreach (var wheel in wheels)
        {
            wheel.Steer(steer);
        }
    }

    private void FixedUpdate()
    {


        return;
        // F_traction = u * EngineForce / F_brake = -u * BrakePower
        Vector3 F_traction = transform.forward * gasPedal * (gasPedal >= 0 ? enginePower : brakePower);
        // F_drag = -C_drag * v * |v|
        Vector3 F_drag = -drag * Rigidbody.velocity.normalized * Rigidbody.velocity.sqrMagnitude;
        // F_rr = -C_rr * v
        Vector3 F_rr = -rollingResistance * Rigidbody.velocity.normalized;
        // Longtitudinal force
        Vector3 F_long = F_traction + F_drag + F_rr;

        Rigidbody.AddForce(F_long);

    }

    public void Reset()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
