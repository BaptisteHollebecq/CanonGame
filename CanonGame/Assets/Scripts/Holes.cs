using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holes : MonoBehaviour
{
    public static event System.Action<Ball> BallFell;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _source.Play();

        if (other.TryGetComponent(out WhiteBall wb))
        {
            wb.Repositioning();
        }
        else if (other.TryGetComponent(out Ball b))
        {
            BallFell?.Invoke(b);
        }
    }
}
