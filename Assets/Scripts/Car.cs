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

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Accelerate(float gasPedal)
    {
        foreach (var wheel in wheels)
        {
            wheel.Accelerate(gasPedal);
        }
    }

    public void Steer(float steer)
    {
        foreach (var wheel in wheels)
        {
            wheel.Steer(steer);
        }
    }

    private void FixedUpdate()
    { 

    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, 200, 20), "Car Velocity: " + Rigidbody.velocity);
        GUI.TextArea(new Rect(10, 30, 200, 20), "Car Speed: " + Rigidbody.velocity.magnitude);

        for (int i = 0; i < wheels.Length; i++)
        {
            int y = 50 + 40 * i;
            GUI.TextArea(new Rect(10, y, 200, 20), string.Format("{0} F_long: {1}", wheels[i].gameObject.name, wheels[i].F_long));
            GUI.TextArea(new Rect(10, y + 20, 200, 20), string.Format("{0} F_lat: {1}", wheels[i].gameObject.name, wheels[i].F_lat));
        }
    }

    public void Reset()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        foreach (var wheel in wheels)
        {
            wheel.carRigidbody.velocity = Vector3.zero;
            wheel.carRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
