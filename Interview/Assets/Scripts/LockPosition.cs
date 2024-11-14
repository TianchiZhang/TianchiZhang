using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPosition : MonoBehaviour
{
    private void LateUpdate()
    {
        if (transform.position != Vector3.zero)
            transform.position = Vector3.zero;
    }
}
