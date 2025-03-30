using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationObject : MonoBehaviour
{
    public float rotationSpeed = 1f; // 회전 속도 (숫자가 클수록 빠름)
    public float rotationAngle = 10f; // 회전 각도 (최대 10도)

    void Update()
    {
        float zRotation = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }
}
