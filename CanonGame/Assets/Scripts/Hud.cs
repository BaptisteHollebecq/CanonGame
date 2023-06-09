using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Hud : MonoBehaviour
{
    public static event System.Action Play;
    public static event System.Action Pause;

    [Header("Menu")]
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private Transform buttons;
    [SerializeField]
    private Transform pointer;
    [SerializeField]
    private TextMeshProUGUI victoryText;

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
    bool paused = false;
    bool won = false;

    int menuIndex = 0;
    bool started = false;

    private void Awake()
    {
        HoleCam.PovHole += PovHole;

        powerBar.gameObject.SetActive(false);
        povCam.gameObject.SetActive(true);
        localPovCamStartingPosition = povCam.transform.localPosition;
        povCam.transform.DOLocalMoveX(1250, 0);

        victoryText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        HoleCam.PovHole -= PovHole;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && started && !won)
        {
            if (!paused)
            {
                Pause?.Invoke();
                menu.SetActive(true);
                buttons.GetChild(1).gameObject.SetActive(true);
                ZenitCamera.SetActive(false);
                paused = true;
            }
            else
            {
                menu.SetActive(false);
                ZenitCamera.SetActive(true);
                Play?.Invoke();
                paused = false;
            }
        }

        // Detect player menu input only if the menu is displayed
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

    // Set up the win screen menu
    public void GameWon(int shots)
    {
        if (!won)
        {
            won = true;
            Pause?.Invoke();
            menu.SetActive(true);
            buttons.GetChild(0).gameObject.SetActive(false);
            buttons.GetChild(1).gameObject.SetActive(true);
            ZenitCamera.SetActive(false);
            victoryText.gameObject.SetActive(true);
            title.SetActive(false);
            victoryText.SetText("Well played, you won in " + shots + " shots !");

            menuIndex = 1;
            ActualisePointer();
        }
    }

    //Actualise the visualiser for menu selection's position
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
