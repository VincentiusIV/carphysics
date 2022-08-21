using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    public Car car;

    private void Awake()
    {
        if(car == null)
            car = GetComponent<Car>();
        Debug.Assert(car);
    }

    private void Update()
    {
        float steer = Input.GetAxisRaw("Horizontal");
        float gasPedal = Input.GetAxisRaw("Vertical");

        car.Accelerate(gasPedal);
        car.Steer(steer);

        if (Input.GetButtonDown("Gear Up"))
        {
            car.GearShift(1);
        }
        if (Input.GetButtonDown("Gear Down"))
        {
            car.GearShift(-1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            car.Reset();
        }
    }
}
