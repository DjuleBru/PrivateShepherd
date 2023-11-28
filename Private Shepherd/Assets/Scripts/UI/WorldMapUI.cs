using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldMapUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI bonesText;
    [SerializeField] private GameObject pauseMenuUI;
    private bool gamePaused;

    public void Start() {
        Player.Instance.OnBonesChanged += Player_OnBonesChanged;
        bonesText.text = Player.Instance.GetPlayerBones().ToString();

        GameInput.Instance.OnPausePerformed += GameInput_OnPausePerformed;
        pauseMenuUI.SetActive(false);
    }

    private void GameInput_OnPausePerformed(object sender, System.EventArgs e) {
        PauseOrUnPause();
    }

    private void Player_OnBonesChanged(object sender, System.EventArgs e) {
        bonesText.text = Player.Instance.GetPlayerBones().ToString();
    }

    public void PauseOrUnPause() {
        if (gamePaused) {
            Time.timeScale = 1.0f;
            pauseMenuUI.SetActive(false);
            gamePaused = false;
        }
        else {
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
            gamePaused = true;
        }
    }
}
