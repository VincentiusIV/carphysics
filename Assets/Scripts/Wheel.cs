using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float radius = 0.43f;
    [Range(1, 100)]
    public int rays = 20;

    private void OnDrawGizmosSelected()
    {
        float anglePerRay = 360 / rays;
        /*for (int i = 0; i < length; i++)
        {

        }*/
    }

    public float enginePower, brakePower, steerPower;
    public float drag = 0.05f, rollingResistance = 1.5f; // rolling resistance should be about 30x drag

    private Rigidbody myRigidbody;
    private float steer, gasPedal;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
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
        if (gasPedal > 0)
        {
            myRigidbody.AddTorque(transform.right * gasPedal * enginePower);
        }
        else
        {
            myRigidbody.AddTorque(transform.right * gasPedal * brakePower * myRigidbody.angularVelocity.x);
        }
        //Vector3 F_gas = transform.right * gasPedal * enginePower * radius;
        // F_traction = u * EngineForce / F_brake = -u * BrakePower
        Vector3 F_traction = transform.forward * gasPedal * (gasPedal >= 0 ? enginePower : brakePower);
        // F_drag = -C_drag * v * |v|
        Vector3 F_drag = -drag * myRigidbody.velocity.normalized * myRigidbody.velocity.sqrMagnitude;
        // F_rr = -C_rr * v
        Vector3 F_rr = -rollingResistance * myRigidbody.velocity.normalized;
        // Longtitudinal force
        Vector3 F_long = F_traction + F_drag + F_rr;

        //myRigidbody.AddForce(F_long);

    }

    public void Reset()
    {
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

}
