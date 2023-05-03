using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event System.Action<Vector3> Shooted;
    public enum State { Aiming, Shooting, Waiting }

    [Header("Setup")]
    [SerializeField]
    private Transform wBall;
    [SerializeField]
    private GameObject cameraShot;
    [SerializeField]
    private GameObject cameraFollow;
    [SerializeField]
    private CameraController camController;

    [Header("Values")]
    [SerializeField]
    private float aimMaxAngle = 60;
    [SerializeField]
    private float angleStep = 3;
    [SerializeField]
    private float shotMaxPower = 100;
    [SerializeField]
    private float slowMoAmonth = 2;

    private State _currentState = State.Aiming;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        WhiteBall.Impact += Impact;

        _rigidbody = wBall.GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        WhiteBall.Impact -= Impact;
        
    }

    private void Aim()
    {
        Vector3 shotForward = cameraShot.transform.forward.normalized;
        int multiUp = shotForward.y >= 0 ? 1 : -1;
        shotForward.y = 0;

        float shootingAngle = multiUp * Mathf.Floor(Vector3.Angle(shotForward, cameraShot.transform.forward));

        if (Input.GetKey(KeyCode.LeftArrow))
            cameraShot.transform.Rotate(new Vector3(0, -angleStep, 0), Space.World);

        if (Input.GetKey(KeyCode.RightArrow))
            cameraShot.transform.Rotate(new Vector3(0, angleStep, 0), Space.World);

        if (Input.GetKey(KeyCode.UpArrow) && shootingAngle + angleStep <= aimMaxAngle)
            cameraShot.transform.Rotate(new Vector3(-angleStep, 0, 0));

        if (Input.GetKey(KeyCode.DownArrow) && shootingAngle >= 0)
            cameraShot.transform.Rotate(new Vector3(angleStep, 0, 0));
    }

    private void Shot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(cameraShot.transform.forward.normalized * shotMaxPower, ForceMode.Impulse);
            Shooted?.Invoke(cameraShot.transform.forward.normalized);
            SwitchState(State.Shooting);
        }
    }

    private void Impact(Vector3 impactPoint)
    {
        if (_currentState != State.Waiting)
        {
            SwitchState(State.Waiting);
            camController.GetRightImpactCamera(impactPoint);
        }
    }

    private void FixedUpdate()
    {
        if (_currentState == State.Aiming)
        {
            Aim();
        }
    }

    private void Update()
    {
        Debug.Log(Time.timeScale);
        if (_currentState == State.Aiming)
        {
            Shot();
        }
    }

    private void SwitchState(State newState)
    {
        switch(newState)
        {
            case State.Shooting:
                {
                    camController.SwitchState(newState);
                    Time.timeScale = 1 / slowMoAmonth;
                    break;
                }
            case State.Waiting:
                {
                    Time.timeScale = 1;
                    break;
                }
        }
        _currentState = newState;
    }
}
