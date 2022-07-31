using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoSingleton<MenuController>
{
    [Header("Transition Options")]
    [SerializeField]
    private float menuTransitionSpeed = 10.0f;
    [SerializeField]
    private float trasitionThreshold = 0.1f;

    [Header("Panel Hiding Position")]
    [SerializeField]
    private Vector3 panelHidePosition;

    [Header("UI Panels")]
    [SerializeField]
    private RectTransform mainMenuPanel;
    [SerializeField]
    private RectTransform difficultyPanel;
    [SerializeField]
    private RectTransform gamePanel;
    [SerializeField]
    private RectTransform scorePanel;
    [SerializeField]
    private RectTransform pausePanel;
    [SerializeField]
    private RectTransform settingsPanel;

    private Vector3 panelShowPosition = Vector3.zero;
    private bool transitionActive = false;
    private RectTransform hidingPanel, showingPanel;
    private bool globalLock = false;

    void Start(){
        mainMenuPanel.localPosition = panelShowPosition;
        difficultyPanel.localPosition = panelHidePosition;
        gamePanel.localPosition = panelHidePosition;
        scorePanel.localPosition = panelHidePosition;
        settingsPanel.localPosition = panelHidePosition;
    }
    

    void Update()
    {
        if (transitionActive)
        {
            if (hidingPanel.localPosition != panelHidePosition && showingPanel.localPosition != panelShowPosition)
            {
                hidingPanel.localPosition = (Vector3.Distance(hidingPanel.localPosition, panelHidePosition) < trasitionThreshold) ? panelHidePosition : Vector3.Lerp(hidingPanel.localPosition, panelHidePosition, Time.deltaTime * menuTransitionSpeed);
                showingPanel.localPosition = (Vector3.Distance(showingPanel.localPosition, panelShowPosition) < trasitionThreshold) ? panelShowPosition : Vector3.Lerp(showingPanel.localPosition, panelShowPosition, Time.deltaTime * menuTransitionSpeed);
            }
            else
            {
                hidingPanel.gameObject.SetActive(false);
                transitionActive = false;
                globalLock = false;
            }
        }
    }

    public void startTransition(RectTransform sourcePanel, RectTransform targetPanel)
    {
        hidingPanel = sourcePanel;
        showingPanel = targetPanel;
        hidingPanel.gameObject.SetActive(true);
        showingPanel.gameObject.SetActive(true);
        transitionActive = true;
        globalLock = true;
    }

    public void MainPanelButtonPlay()
    {
        if (!globalLock)
        {
            startTransition(mainMenuPanel, difficultyPanel);
        }
    }

    public void MainPanelButtonSettings()
    {
        if (!globalLock)
        {
            startTransition(mainMenuPanel, settingsPanel);
        }
    }

    public void MainPanelButtonExit()
    {
        if (!globalLock)
        {
            Application.Quit();
        }
    }

    public void DifficultyPanelButton3X2()
    {
        if (!globalLock)
        {
            GameController.Instance.GameStart(3, 2);
            startTransition(difficultyPanel, gamePanel);
        }
    }

    public void DifficultyPanelButton4X3()
    {
        if (!globalLock)
        {
            GameController.Instance.GameStart(4, 3);
            startTransition(difficultyPanel, gamePanel);
        }
    }

    public void DifficultyPanelButton5X4()
    {
        if (!globalLock)
        {
            GameController.Instance.GameStart(5, 4);
            startTransition(difficultyPanel, gamePanel);
        }
    }

    public void DifficultyPanelButtonBack()
    {
        if (!globalLock)
        {
            startTransition(difficultyPanel, mainMenuPanel);
        }
    }

    public void GamePanelButtonBack()
    {
        if (!globalLock)
        {
            GameController.Instance.GameStop();
            startTransition(gamePanel, mainMenuPanel);
        }
    }

    public void GamePanelButtonPause()
    {
        if (!globalLock)
        {
            GameController.Instance.GamePause();
            pausePanel.gameObject.SetActive(true);
        }
    }

    public void ScorePanelButtonReplay()
    {
        if (!globalLock)
        {
            startTransition(scorePanel, difficultyPanel);
        }
    }

    public void ScorePanelButtonBack()
    {
        if (!globalLock)
        {
            startTransition(scorePanel, mainMenuPanel);
        }
    }

    public void PausePanelButtonResume()
    {
        pausePanel.gameObject.SetActive(false);
        GameController.Instance.GameResume();
    }

    public void PausePanelButtonExit()
    {
        pausePanel.gameObject.SetActive(false);
        GameController.Instance.GameStop();
        startTransition(gamePanel, mainMenuPanel);
    }

    public void SettingsPanelButtonResetScores()
    {
        PlayerPrefs.DeleteAll();
        startTransition(settingsPanel, mainMenuPanel);
    }

    public void SettingsPanelButtonBack()
    {
        startTransition(settingsPanel, mainMenuPanel);
    }
}
