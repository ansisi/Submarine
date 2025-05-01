using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCameraMove : MonoBehaviour
{
    public float rotationSpeed = 2f;
    public Vector3 rotationAxis = Vector3.up; // Y축 중심으로 회전

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
