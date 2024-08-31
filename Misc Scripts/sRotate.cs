using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotSpeed;
    private void LateUpdate()
    {
        transform.Rotate(rotSpeed * Time.deltaTime);
    }
}
