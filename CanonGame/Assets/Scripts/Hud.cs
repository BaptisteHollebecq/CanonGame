using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [SerializeField]
    private Image powerBar;
    [SerializeField]
    private Transform cams;

    Dictionary<Transform, float> nearestBall = new Dictionary<Transform, float>();
    bool[] povs = new bool[6];

    private void Awake()
    {
        HoleCam.PovHole += PovHole;

        powerBar.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        HoleCam.PovHole -= PovHole;
    }


    private void PovHole(int index, float distance)
    {

    }


    public void SwitchPowerBar()
    {
        powerBar.gameObject.SetActive(!powerBar.gameObject.activeSelf);
    }

    public void ActualisePowerBar(float power)
    {
        powerBar.fillAmount = power;
    }
}
