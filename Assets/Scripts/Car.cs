using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Wheel[] wheels;
    private Rigidbody Rigidbody;

    public AnimationCurve torqueCurve;
    private float minRPM, maxRPM;

    public float ratio = 10; // Temporary shortcut for gear ratio, differential ratio, etc.
    private float rpm = 0;
    private float clampedrpm = 0;
    private float referenceWheelAngVel; // Rad/s of reference wheel to calculate engine rpm

    private const float rads2rpm = 60 / (2 * Mathf.PI); // TODO: put this somewhere cleaner

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        minRPM = torqueCurve.keys[0].time;
        maxRPM = torqueCurve.keys[torqueCurve.length - 1].time;
    }

    private void FixedUpdate()
    {
        referenceWheelAngVel = transform.InverseTransformDirection(Rigidbody.velocity).z / wheels[0].radius;
        rpm = referenceWheelAngVel * ratio * rads2rpm;
        clampedrpm = Mathf.Clamp(rpm, minRPM, maxRPM);
    }

    public void Accelerate(float gasPedal)
    {
        foreach (var wheel in wheels)
        {
            wheel.Accelerate(torqueCurve.Evaluate(clampedrpm) * Mathf.Max(gasPedal, 0));
            wheel.Brake(gasPedal);
        }
    }

    public void Steer(float steer)
    {
        foreach (var wheel in wheels)
        {
            wheel.Steer(steer);
        }
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, 200, 20), "Car Velocity: " + Rigidbody.velocity);
        GUI.TextArea(new Rect(10, 30, 200, 20), "Car Speed: " + Rigidbody.velocity.magnitude);

        GUI.TextArea(new Rect(10, 50, 200, 20), "Car RPM: " + string.Format("{0:0.000}", rpm));
        GUI.TextArea(new Rect(10, 70, 200, 20), "Car Clamped RPM: " + string.Format("{0:0.000}", clampedrpm));

        for (int i = 0; i < wheels.Length; i++)
        {
            int y = 90 + 40 * i;
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
