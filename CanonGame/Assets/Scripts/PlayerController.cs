using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event System.Action<Vector3> Shooted;
    public enum State { Menu, Aiming, Shooting, Waiting }

    [Header("Setup")]
    [SerializeField]
    private WhiteBall wBall;
    [SerializeField]
    private GameObject cameraShot;
    [SerializeField]
    private GameObject cameraFollow;
    [SerializeField]
    private CameraController camController;
    [SerializeField]
    private Hud hud;
    [SerializeField]
    private Transform impactPoint;
    [SerializeField]
    private LayerMask impactDetection;
    [SerializeField]
    private List<Ball> balls = new List<Ball>();

    [Header("Values")]
    [SerializeField]
    private float aimMaxAngle = 60;
    [SerializeField]
    private float angleStep = 3;
    [SerializeField]
    private float shotMaxPower = 100;
    [SerializeField]
    private float shotChargingMultiplier = 1;
    [SerializeField]
    private float slowMoAmonth = 2;

    private State _currentState = State.Menu;
    private Rigidbody _rigidbody;
    private bool _firing = false;
    private float _power = 0;
    private int _switchPower = 1;
    private int _shots = 0;

    private AudioSource _source;

    private void Awake()
    {
        WhiteBall.Impact += Impact;
        Holes.BallFell += RemoveBall;
        Hud.Play += Play;
        Hud.Pause += Pause;

        _rigidbody = wBall.GetComponent<Rigidbody>();
        _source = GetComponent<AudioSource>();
        impactPoint.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        WhiteBall.Impact -= Impact;
        Holes.BallFell -= RemoveBall;
        Hud.Play -= Play;
        Hud.Pause -= Pause;
    }

    private void Play()
    {
        StartCoroutine(SwitchState(State.Aiming));
    }

    private void Pause()
    {
        StartCoroutine(SwitchState(State.Menu));
    }

    public void RemoveBall(Ball obj)
    {
        balls.Remove(obj);
        Destroy(obj.gameObject);
    }

    private void Aim()
    {
        // Some calculs to clamp vision
        Vector3 shotForward = cameraShot.transform.forward.normalized;
        int multiUp = shotForward.y >= 0 ? 1 : -1;
        shotForward.y = 0;

        float shootingAngle = multiUp * Mathf.Floor(Vector3.Angle(shotForward, cameraShot.transform.forward));

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            cameraShot.transform.Rotate(new Vector3(0, -angleStep, 0), Space.World);
            cameraFollow.transform.Rotate(new Vector3(0, -angleStep, 0), Space.World);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            cameraShot.transform.Rotate(new Vector3(0, angleStep, 0), Space.World);
            cameraFollow.transform.Rotate(new Vector3(0, angleStep, 0), Space.World);
        }

        if (Input.GetKey(KeyCode.UpArrow) && shootingAngle + angleStep <= aimMaxAngle)
            cameraShot.transform.Rotate(new Vector3(-angleStep, 0, 0));

        if (Input.GetKey(KeyCode.DownArrow) && shootingAngle >= 0)
            cameraShot.transform.Rotate(new Vector3(angleStep, 0, 0));

        // Detect where the ball gonna touch first (can be improved A LOT)
        RaycastHit hit;
        Vector3 visee = cameraShot.transform.forward.normalized;
        visee.y = 0;
        if (Physics.SphereCast(wBall.transform.position, 0.0342f, visee, out hit, 10, impactDetection))
        {
            impactPoint.gameObject.SetActive(true);
            impactPoint.transform.position = hit.point;
            Vector3 forward = hit.normal;
            forward.y = 0;
            impactPoint.forward = forward;
        }
        else
            impactPoint.gameObject.SetActive(false);
    }

    private void Shot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _firing = true;
            hud.SwitchPowerBar(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _firing = false;
            hud.SwitchPowerBar(false);
            if (_power != 0)
            {
                _rigidbody.AddForce(cameraShot.transform.forward.normalized * shotMaxPower * _power, ForceMode.Impulse);
                Shooted?.Invoke(cameraShot.transform.forward.normalized);
                //Change power after first shot to keep a more precise gameplay
                if (_shots == 0)
                    shotMaxPower = 1;
                _shots++;
                _source.Play();
                StartCoroutine(SwitchState(State.Shooting));
            }
            _power = 0;
        }

        if (_firing)
        {
            _power += Time.deltaTime * shotChargingMultiplier * _switchPower;
            if (_power > 1 || _power < 0)
                _switchPower *= -1;

            hud.ActualisePowerBar(_power);
        }
    }

    private void Impact(Vector3 impactPoint)
    {
        if (_currentState == State.Shooting)
        {
            StartCoroutine(SwitchState(State.Waiting));
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
        if (_currentState == State.Aiming)
        {
            Shot();
        }
        else if (_currentState == State.Waiting || _currentState == State.Shooting)
        {
            int moving = 0;
            foreach (Ball b in balls)
            {
                if (b.velocityMagnitude != 0)
                    moving++;
            }
            // If in the right state and no ball moving switch to aim state
            if (moving == 0)
                StartCoroutine(SwitchState(State.Aiming));
        }

        if (balls.Count == 1 && balls[0].TryGetComponent(out WhiteBall wb))
        {
            hud.GameWon(_shots);
            // WIN
        }
    }

    private IEnumerator SwitchState(State newState)
    {
        yield return new WaitForSeconds(0.05f);
        switch (newState)
        {
            case State.Aiming:
                {
                    impactPoint.gameObject.SetActive(true);
                    camController.SwitchState(newState);
                    break;
                }
            case State.Shooting:
                {
                    impactPoint.gameObject.SetActive(false);
                    camController.SwitchState(newState);
                    Time.timeScale = 1 / slowMoAmonth;
                    break;
                }
            case State.Waiting:
                {
                    Time.timeScale = 1;
                    break;
                }
            case State.Menu:
                {
                    camController.SwitchState(newState);
                    break;
                }
        }
        _currentState = newState;
    }
}
