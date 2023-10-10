using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRunVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem tiredPS;
    [SerializeField] private ParticleSystem extremelyTiredPS;
    [SerializeField] private Image runProgressionBar;

    private bool playingTiredPS;
    private bool playingExtremelyTiredPS;

    private void Start() {
        PlayerRun.Instance.onPlayerRun += PlayerRun_onPlayerRun;
        PlayerRun.Instance.onPlayerStopRun += PlayerRun_onPlayerStopRun;

    }

    private void Update() {

        runProgressionBar.fillAmount = PlayerRun.Instance.GetRunProgression();

        if (PlayerRun.Instance.GetTired() & !PlayerRun.Instance.GetExtremelyTired()) {
            if(!playingTiredPS) {
                tiredPS.Play();
                playingTiredPS = true;
            }
            runProgressionBar.color = Color.yellow;
        }
        else {
            runProgressionBar.color = Color.green;
            if (playingTiredPS) {
                tiredPS.Stop();
                playingTiredPS = false;
            }
        }

        if (PlayerRun.Instance.GetExtremelyTired()) {
            if (!playingExtremelyTiredPS) {
                extremelyTiredPS.Play();
                playingExtremelyTiredPS = true;
            }
            runProgressionBar.color = Color.red;
        } else {
            extremelyTiredPS.Stop();
            playingExtremelyTiredPS = false;
        }

        
    }

    private void PlayerRun_onPlayerStopRun(object sender, System.EventArgs e) {
        
    }

    private void PlayerRun_onPlayerRun(object sender, System.EventArgs e) {
       
    }
}
