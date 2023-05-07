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

    // If there is a ball near the hole send information to the HUD
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
        // Find the nearest ball if there is multiple ball in the zone
        foreach (Ball b in balls)
        {
            if (b.velocityMagnitude != 0 && (b.transform.position - transform.position).magnitude < (nearest.transform.position - transform.position).magnitude)
                nearest = b;
        }

        // If not live and there is a ball moving turn on Live
        if (!live && nearest.velocityMagnitude != 0)
            OnAir((nearest.transform.position - transform.position).magnitude);

        // If live but the bal ain't moving turn off live
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
