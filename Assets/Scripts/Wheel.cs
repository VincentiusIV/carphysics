using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float radius = 1;
    [Range(1, 100)]
    public int rays = 20;

    private void OnDrawGizmosSelected()
    {
        float anglePerRay = 360 / rays;
        for (int i = 0; i < length; i++)
        {

        }
    }

}
