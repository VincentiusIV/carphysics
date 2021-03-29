using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offsetFromTarget;
    public float speed = 5, rotateSpeed = 10;
    public float minY = 2;
    public bool fixedUpdate = true;

    private void Update()
    {
        if (!Application.isPlaying || !fixedUpdate)
            UpdateCameraOrientation();
    }

    private void FixedUpdate()
    {
        if(fixedUpdate)
            UpdateCameraOrientation();
    }

    private void UpdateCameraOrientation()
    {
        if (target == null) return;
        Vector3 targetPosition = target.TransformPoint(offsetFromTarget);
        targetPosition.y = Mathf.Max(minY, targetPosition.y);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Mathf.Clamp01(Time.fixedDeltaTime * speed));

        Vector3 toTarget = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(toTarget, Vector3.up), Mathf.Clamp01(Time.fixedDeltaTime * rotateSpeed));
    }
}
