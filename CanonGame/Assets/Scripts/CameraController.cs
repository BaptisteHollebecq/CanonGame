using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraShot;
    [SerializeField]
    private GameObject cameraFollow;
    [SerializeField]
    private GameObject cameraImpact;
    [SerializeField]
    private CinemachineBrain brain;

    private void Awake()
    {
        cameraShot.SetActive(true);
        cameraFollow.SetActive(false);
        cameraImpact.SetActive(false);
    }

    public void GetRightImpactCamera(Vector3 impactPoint)
    {
        brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

        cameraShot.SetActive(false);
        cameraFollow.SetActive(false);

        cameraImpact.transform.position = new Vector3(impactPoint.x, 1, (impactPoint.z >= 0)? .5f : -.5f);
        cameraImpact.transform.rotation = Quaternion.LookRotation((impactPoint - cameraImpact.transform.position).normalized, Vector3.up);

        cameraImpact.gameObject.SetActive(true);

    }

    public void SwitchState(PlayerController.State newState)
    {
        switch (newState)
        {
            case PlayerController.State.Shooting:
                {
                    cameraShot.SetActive(false);
                    cameraFollow.SetActive(true);
                    break;
                }
        }
    }
}
