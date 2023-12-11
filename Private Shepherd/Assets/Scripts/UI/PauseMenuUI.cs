using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject settingsPanel;

    private bool settingsOpen;

    private void Start() {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void OnEnable() {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OpenSettings() {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings() {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }


}
