using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HoleCam : MonoBehaviour
{
    public static event System.Action<int, float> PovHole;

    [SerializeField]
    private int index;

    List<Ball> balls = new List<Ball>();
    Ball nearest = null;
    public bool live = false;

    private void Awake()
    {
        Holes.BallFell += RemoveBall;
    }

    void OnDestroy()
    {
        Holes.BallFell -= RemoveBall;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball b))
        {
            balls.Add(b);
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
        else
        {
            nearest = null;
            if (live)
                OnAir(99);
            return;
        }

        foreach (Ball b in balls)
        {
            if (b.velocityMagnitude != 0 && (b.transform.position - transform.position).magnitude < (nearest.transform.position - transform.position).magnitude)
                nearest = b;
        }

        if (!live && nearest.velocityMagnitude != 0)
            OnAir((nearest.transform.position - transform.position).magnitude);

        if (live && nearest.velocityMagnitude == 0)
            OnAir((nearest.transform.position - transform.position).magnitude);
    }

    private void RemoveBall(Ball b)
    {
        if (live && nearest == b)
            OnAir(10);
        balls.Remove(b);
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
