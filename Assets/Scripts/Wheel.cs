using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float radius;
    public float staticFriction;
    public float kineticFriction;

    public float enginePower, brakePower, steerPower;
    public float drag;
    [Tooltip("Rolling resistance should be about 30x drag")]
    public float rollingResistance; 

    private Rigidbody myRigidbody;
    private float steer, gasPedal;

    private ParticleSystem particles;

    public void Reset()
    {
        radius = .5f;
        staticFriction = 1;
        kineticFriction = 0.7f;
        
        enginePower = 50;
        brakePower = 10;
        drag = 0.05f;
        rollingResistance = 1.5f; // rolling resistance should be about 30x drag
        // Wikipedia says the roll resistance of tyres on concrete should be 0.01 lol

        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody)
        {
            myRigidbody.sleepThreshold = 0.0f; // If the rigidbody goes to sleep we don't get collisions
        }
        particles = GetComponent<ParticleSystem>();
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
        myRigidbody.MoveRotation(Quaternion.Euler(0, steer * 25, 0));

        /*
        if (gasPedal > 0)
        {
            myRigidbody.AddTorque(transform.right * gasPedal * enginePower);
        }
        else
        {
            myRigidbody.AddTorque(transform.right * gasPedal * brakePower * myRigidbody.angularVelocity.x);
        }*/
        //Vector3 F_gas = transform.right * gasPedal * enginePower * radius;
        // F_traction = u * EngineForce / F_brake = -u * BrakePower
        //Vector3 F_traction = transform.forward * gasPedal * (gasPedal >= 0 ? enginePower : brakePower);
        // F_drag = -C_drag * v * |v|
        //Vector3 F_drag = -drag * myRigidbody.velocity.normalized * myRigidbody.velocity.sqrMagnitude;
        // F_rr = -C_rr * v
        //Vector3 F_rr = -rollingResistance * myRigidbody.velocity.normalized;
        // Longtitudinal force
        //Vector3 F_long = F_traction + F_drag + F_rr;

        //myRigidbody.AddForce(F_long);

    }

    void OnCollisionStay(Collision collisionInfo)
    {
        //float normalForceMagnitude = 9.81f * myRigidbody.mass;
        //myRigidbody.AddForce(Vector3.Cross(transform.right, collisionInfo.contacts[0].normal) * fric * normalForceMagnitude);
        //Debug.Log(collisionInfo.contactCount);
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
            
        }

        float F_long = 0;
        if (gasPedal > 0)
        {
            float f_gas = gasPedal * enginePower * radius;
            if (f_gas <= staticFriction * Physics.gravity.magnitude * myRigidbody.mass)
            {
                F_long = f_gas;
            }
            else
            {
                Debug.Log("Wheelspin");
                if (!particles.isEmitting)
                {
                    particles.Play();
                }
                F_long = kineticFriction * 9.81f * myRigidbody.mass;
            }
        }
        else
        {
            if (gasPedal < 0)
            {
                F_long = gasPedal * brakePower * Mathf.Sign(myRigidbody.velocity.z);
            }
        }
        float xVel = transform.InverseTransformDirection(myRigidbody.velocity).x;
        float F_lat = kineticFriction * 9.81f * myRigidbody.mass * -Mathf.Sign(xVel);
        // Friction force is constant, we need to ensure the velocity change during the next frame doesn't exceed the velocity causing the friction
        F_lat = Mathf.Clamp(F_lat, -Mathf.Abs(xVel) * myRigidbody.mass / Time.fixedDeltaTime, Mathf.Abs(xVel) * myRigidbody.mass / Time.fixedDeltaTime);
        Vector3 F_traction = transform.forward * F_long + transform.right * F_lat;
        myRigidbody.AddForce(F_traction);
    }

}
