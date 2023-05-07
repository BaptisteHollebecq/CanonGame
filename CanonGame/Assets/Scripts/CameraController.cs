using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraMenu;
    [SerializeField]
    private GameObject cameraShot;
    [SerializeField]
    private GameObject cameraFollow;
    [SerializeField]
    private GameObject cameraImpact;
    [SerializeField]
    private GameObject cameraZenit;
    [SerializeField]
    private CinemachineBrain brain;
    [SerializeField]
    private Transform holesCam;


    private void Awake()
    {
        cameraShot.SetActive(false);
        cameraFollow.SetActive(false);
        cameraImpact.SetActive(false);
    }

    public void GetRightImpactCamera(Vector3 impactPoint)
    {
        // Cinemachine blend => cut for instant change
        brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

        cameraShot.SetActive(false);
        cameraFollow.SetActive(false);

        // Place the impactCamera on the right position (z/-z => wich side of the table)
        cameraImpact.transform.position = new Vector3(impactPoint.x, 1, (impactPoint.z >= 0)? .5f : -.5f);
        cameraImpact.transform.rotation = Quaternion.LookRotation((impactPoint - cameraImpact.transform.position).normalized, Vector3.up);
        cameraImpact.gameObject.SetActive(true);

    }

    // Set the right Cameras and their settings depending on the state
    public void SwitchState(PlayerController.State newState)
    {
        switch (newState)
        {
            case PlayerController.State.Aiming:
                {
                    brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Linear;
                    brain.m_DefaultBlend.m_Time = 1f;
                    cameraImpact.gameObject.SetActive(false);
                    cameraMenu.gameObject.SetActive(false);
                    cameraShot.SetActive(true);
                    cameraFollow.SetActive(false);
                    break;
                }
            case PlayerController.State.Shooting:
                {
                    brain.m_DefaultBlend.m_Time = .1f;
                    cameraShot.SetActive(false);
                    cameraFollow.SetActive(true);
                    break;
                }
            case PlayerController.State.Menu:
                {
                    brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Linear;
                    brain.m_DefaultBlend.m_Time = .5f;
                    cameraImpact.gameObject.SetActive(false);
                    cameraMenu.gameObject.SetActive(true);
                    cameraShot.SetActive(false);
                    cameraFollow.SetActive(false);
                    break;
                }
        }
    }
}
