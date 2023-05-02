using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform wBall;
    [SerializeField]
    private GameObject cameraShot;
    [SerializeField]
    private GameObject cameraFollow;

    private void Awake()
    {
        cameraShot.SetActive(true);
        cameraFollow.SetActive(false);
    }
}
