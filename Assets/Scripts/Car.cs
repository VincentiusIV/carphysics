using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float drag;
    public Wheel[] wheels;
    private Rigidbody Rigidbody;

    [Header("Engine")]
    public AnimationCurve torqueCurve;
    private float minRPM, maxRPM; // derived from torqueCurve

    [Header("Gearing")]
    public float[] gearRatios = { 3.91f, 2.44f, 1.81f, 1.46f, 1.19f, 0.97f};
    public float finalDriveRatio = 2.86f;
    public float reverseRatio = -2.93f;
    public float gearEfficiency = .2f;

    private int currentGear = 0;

    private float rpm = 0;
    private float clampedrpm = 0;
    private float referenceWheelAngVel; // Rad/s of reference wheel to calculate engine rpm

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        minRPM = torqueCurve.keys[0].time;
        maxRPM = torqueCurve.keys[torqueCurve.length - 1].time;
    }

    private void FixedUpdate()
    {
        referenceWheelAngVel = transform.InverseTransformDirection(Rigidbody.velocity).z / wheels[0].radius; // cheating
        rpm = referenceWheelAngVel * gearRatios[currentGear] * finalDriveRatio * Utility.RADPS2RPM;
        clampedrpm = Mathf.Clamp(rpm, minRPM, maxRPM);

        // drag

        Vector3 F_drag = -drag * Rigidbody.velocity.normalized * Rigidbody.velocity.magnitude;
        //Rigidbody.AddForce(F_drag);
    }

    public void Accelerate(float gasPedal)
    {
        foreach (var wheel in wheels)
        {
            if (wheel.isPowered)
            {
                wheel.Accelerate(torqueCurve.Evaluate(clampedrpm) * Mathf.Max(gasPedal, 0) * gearRatios[currentGear] * finalDriveRatio * gearEfficiency);
            }
            else
            {
                wheel.Accelerate(0);
            }
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

    public void GearShift(int change)
    {
        if (0 <= currentGear + change && currentGear + change < gearRatios.Length)
        {
            currentGear += change;
        }
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, 200, 20), "Car Velocity: " + Rigidbody.velocity);
        GUI.TextArea(new Rect(10, 30, 200, 20), "Car Speed: " + Rigidbody.velocity.magnitude);
        GUI.TextArea(new Rect(10, 50, 200, 20), "km/h: " + Rigidbody.velocity.magnitude * 3.6f);

        GUI.TextArea(new Rect(10, 70, 200, 20), "Car RPM: " + string.Format("{0:0.00}", clampedrpm));
        GUI.TextArea(new Rect(10, 90, 200, 20), "Gear: " + string.Format("{0}", currentGear + 1));

        for (int i = 0; i < wheels.Length; i++)
        {
            int y = 110 + 40 * i;
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
            wheel.ClearVelocities();
        }
    }
}
