using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offsetFromTarget;
    public float speed = 5, rotateSpeed = 10;
    public bool fixedUpdate = true;
    public float minY = -30, maxY = 30;
    public Vector2 sensitivity = new Vector2(5, 5);
    public float zoomSpeed = 5;
    public float minDist = 2, maxDist = 10f;

    private float distance;
    private float x, y;

    private void Update()
    {
        if (!Application.isPlaying || !fixedUpdate)
            UpdateCameraOrientation(Time.unscaledDeltaTime);
    }

    private void FixedUpdate()
    {
        if(fixedUpdate)
            UpdateCameraOrientation(Time.fixedDeltaTime);
    }

    private void UpdateCameraOrientation(float deltaTime)
    {
        if (target == null) return;
        /*Vector3 targetPosition = target.TransformPoint(offsetFromTarget);
        targetPosition.y = Mathf.Max(minY, targetPosition.y);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Mathf.Clamp01(Time.fixedDeltaTime * speed));
        Vector3 toTarget = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(toTarget, Vector3.up), Mathf.Clamp01(Time.fixedDeltaTime * rotateSpeed));
        */
        
        x += Input.GetAxisRaw("Mouse X") * sensitivity.x * deltaTime;
        y -= Input.GetAxisRaw("Mouse Y") * sensitivity.y * deltaTime;
        y = ClampAngle(y, minY, maxY);

        distance -= Input.mouseScrollDelta.y * zoomSpeed * deltaTime;
        distance = Mathf.Clamp(distance, minDist, maxDist);

        Quaternion targetRotation = Quaternion.Euler(y, x, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        
        Vector3 targetPosition = targetRotation * negDistance + target.position + (targetRotation * (offsetFromTarget));

        transform.rotation = targetRotation;
        transform.position = targetPosition;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle %= 360f;
        if (angle < -360f)
            angle += 360f;
        return Mathf.Clamp(angle, min, max);
    }

    public static float ClampAngle180(float angle, float min, float max)
    {
        angle %= 180f;
        return Mathf.Clamp(angle, min, max);
    }
}
