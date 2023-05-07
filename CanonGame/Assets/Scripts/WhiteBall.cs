using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBall : Ball
{
    public static event System.Action<Vector3> Impact;

    [HideInInspector]
    public Vector3 startingPos;

    private new void Awake()
    {
        base.Awake();

        startingPos = transform.position;
    }

    private new void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.transform.TryGetComponent(out Ball b) && b != this)
            StartCoroutine(Impacted(collision.transform.position));
    }

    // Delayed collision notification
    IEnumerator Impacted(Vector3 pos)
    {
        yield return new WaitForSeconds(0.05f);
        Impact?.Invoke(pos);
    }

    // Teleport at starting position
    public void Repositioning()
    {
        _rigidbody.velocity = Vector3.zero;
        transform.position = startingPos;
    }

}
