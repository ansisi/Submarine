using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundery : MonoBehaviour
{
    void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Resource")) // 아이템 태그가 있는 경우 삭제
        {
            Destroy(other.gameObject);
        }
    }
}
