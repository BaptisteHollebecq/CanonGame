using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Hud : MonoBehaviour
{
    public static event System.Action Play;
    public static event System.Action Pause;

    [Header("Menu")]
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private Transform buttons;
    [SerializeField]
    private Transform pointer;

    [Header("Game")]
    [SerializeField]
    private GameObject ZenitCamera;
    [SerializeField]
    private Image powerBar;
    [SerializeField]
    private RawImage povCam;
    [SerializeField]
    private List<Texture> cams;

    Dictionary<Texture, float> nearestBall = new Dictionary<Texture, float>();
    bool[] povs = new bool[6];
    bool camShown = false;
    Vector3 localPovCamStartingPosition = Vector3.zero;

    int menuIndex = 0;
    bool started = false;

    private void Awake()
    {
        HoleCam.PovHole += PovHole;

        powerBar.gameObject.SetActive(false);
        povCam.gameObject.SetActive(true);
        localPovCamStartingPosition = povCam.transform.localPosition;
        povCam.transform.DOLocalMoveX(1250, 0);
    }

    private void OnDestroy()
    {
        HoleCam.PovHole -= PovHole;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause?.Invoke();
            menu.SetActive(true);
            buttons.GetChild(1).gameObject.SetActive(true);
            ZenitCamera.SetActive(false);
        }

       if (menu.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                menuIndex--;
                if (menuIndex < 0)
                    menuIndex = buttons.childCount - 1;

                if (!buttons.GetChild(menuIndex).gameObject.activeSelf)
                    menuIndex--;

                ActualisePointer();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                menuIndex++;
                if (menuIndex == buttons.childCount)
                    menuIndex = 0;
                if (!buttons.GetChild(menuIndex).gameObject.activeSelf)
                    menuIndex++;
                ActualisePointer();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (menuIndex)
                {
                    case 0:
                        {
                            PlayTheGame();
                            break;
                        }
                    case 1:
                        {
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                            break;
                        }
                    case 2:
                        {
                            QuitTheGame();
                            break;
                        }
                }
            }
        }
    }

    private void ActualisePointer()
    {
        pointer.SetParent(buttons.GetChild(menuIndex));
        pointer.localPosition = Vector3.zero;
    }

    public void PlayTheGame()
    {
        menu.SetActive(false);
        ZenitCamera.SetActive(true);
        started = true;
        Play?.Invoke();
    }

    private void QuitTheGame()
    {
        Application.Quit();
    }

    private void PovHole(int index, float distance)
    {
        // receive information and actualise data with it
        povs[index] = !povs[index];
        nearestBall[cams[index]] = distance;

        float nearest = 999;
        int bestCam = -1;
        // find if a cam need to be shown
        for (int i = 0; i < povs.Length; i++) 
        {
            if (!povs[i])
                continue;

            if (nearestBall[cams[i]] < nearest)
            {
                nearest = nearestBall[cams[i]];
                bestCam = i;
            }
        }


        if (bestCam != -1)
        {
            if (!camShown)
            {
                // SHOW CAM
                povCam.transform.DOLocalMove(localPovCamStartingPosition, .25f);
                camShown = true;
            }
            //ACTUALISE CAM
            povCam.texture = cams[bestCam];
        }
        else if (camShown)
        {
            // HIDE CAM
            povCam.transform.DOLocalMoveX(1250, .25f);
            camShown = false;
        }
    }

    public void SwitchPowerBar(bool active)
    {
        powerBar.gameObject.SetActive(active);
    }

    public void ActualisePowerBar(float power)
    {
        powerBar.fillAmount = power;
    }
}
