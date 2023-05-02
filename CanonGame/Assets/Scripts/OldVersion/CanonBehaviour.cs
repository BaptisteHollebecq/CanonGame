using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float maxDistance = 20;
    [SerializeField]
    private float maxAngle = 65;

    private Transform pivot = null;

    private void Awake()
    {
        foreach(Transform t in transform)
        {
            if (t.name == "CanonPivot")
                pivot = t;
        }
        if (pivot == null)
            Debug.LogWarning("Error: Canon Pivot's missing.");
    }

    private void Update()
    {
        Vector3 targetDirection = target.position - transform.position;
        float targetDistance = targetDirection.magnitude;

        transform.forward = -targetDirection;
        pivot.localRotation = Quaternion.Euler(-maxAngle * (targetDistance / maxDistance), 0, 0);

    }
}
