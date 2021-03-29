using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private Rigidbody Rigidbody;
    public float accelerationSpeed = 10, steerSpeed = 10;
    private float steer, acceleration;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Accelerate(float acceleration)
    {
        this.acceleration = acceleration;
    }

    public void Steer(float steer)
    {
        this.steer = steer;
    }

    private void FixedUpdate()
    {
        Rigidbody.AddForce(transform.forward * acceleration * accelerationSpeed);
        Rigidbody.AddTorque(Vector3.up * steer * steerSpeed);
    }

    public void Reset()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
