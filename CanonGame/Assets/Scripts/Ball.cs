using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static event System.Action<Vector3> Collided;

    public float velocityMagnitude;

    protected Rigidbody _rigidbody;

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Ball b))
        {
            // Invoke for the sound manager to play collision sound
            Collided?.Invoke(collision.contacts[0].point);
        }
    }

    private void FixedUpdate()
    {
        velocityMagnitude = _rigidbody.velocity.magnitude;
    }

}
