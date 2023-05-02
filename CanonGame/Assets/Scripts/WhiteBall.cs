using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBall : Ball
{
    public static event System.Action<Vector3> Impact;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Ball b) && b != this)
            Impact?.Invoke(collision.contacts[0].point);
    }
}
