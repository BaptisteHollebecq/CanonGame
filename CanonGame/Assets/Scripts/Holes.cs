using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holes : MonoBehaviour
{
    public static event System.Action<Ball> BallFell;

    private void OnTriggerEnter(Collider other)
    {
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
