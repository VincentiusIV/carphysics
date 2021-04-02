using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelInputHandler : MonoBehaviour
{
    public Wheel wheel;

    private void Awake()
    {
        if (wheel == null)
            wheel = GetComponent<Wheel>();
        Debug.Assert(wheel);
    }

    private void Update()
    {
        float steer = Input.GetAxisRaw("Horizontal");
        float acceleration = Input.GetAxisRaw("Vertical");

        wheel.Accelerate(acceleration);
        wheel.Steer(steer);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            wheel.Reset();
        }
    }
}
