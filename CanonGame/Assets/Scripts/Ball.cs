using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float velocityMagnitude;

    protected Rigidbody _rigidbody;

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        velocityMagnitude = _rigidbody.velocity.magnitude;
    }

}
