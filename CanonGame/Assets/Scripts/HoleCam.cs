using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleCam : MonoBehaviour
{
    public static event System.Action<int, float> PovHole;

    [SerializeField]
    private int index;

    List<Ball> balls = new List<Ball>();
    Ball nearest = null;
    bool live = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball b))
        {
            balls.Add(b);
            OnAir((b.transform.position - transform.position).magnitude);
        }
    }

    private void OnAir(float distance)
    {
        if (!live)
        {
            PovHole?.Invoke(index, distance);
            live = true;
        }
        else
        {
            PovHole?.Invoke(index, distance);
            live = false;
        }
    }


    private void Update()
    {
        if (balls.Count != 0)
            nearest = balls[0];
        foreach(Ball b in balls)
        {
            if (b.velocityMagnitude != 0 && (b.transform.position - transform.position).magnitude < (nearest.transform.position - transform.position).magnitude)
                nearest = b;
        }

        if (nearest.velocityMagnitude == 0 && live)
            OnAir((nearest.transform.position - transform.position).magnitude);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Ball b))
        {
            if (balls.Contains(b))
                balls.Remove(b);
        }
    }
}
