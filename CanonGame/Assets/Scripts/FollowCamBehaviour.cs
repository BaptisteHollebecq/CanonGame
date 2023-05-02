using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamBehaviour : MonoBehaviour
{
    private void Awake()
    {
        PlayerController.Shooted += AdjustPosition;
    }

    private void OnDestroy()
    {
        PlayerController.Shooted -= AdjustPosition;
    }

    private void AdjustPosition(Vector3 direction)
    {
        transform.forward = new Vector3(direction.x, 0, direction.z);
    }
}
