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

    private Color runColor = new Color(121f / 255f, 157f / 255f, 0f / 255f, 255f / 255f);
    private Color tiredColor = new Color(215f / 255f, 156f / 255f, 0f / 255f, 255f / 255f);
    private Color extremeColor = new Color(142f / 255f, 62f / 255f, 05f / 255f, 255f / 255f);


    private void Update() {

        runProgressionBar.fillAmount = 1 - PlayerRun.Instance.GetRunProgression();

        if (PlayerRun.Instance.GetTired() & !PlayerRun.Instance.GetExtremelyTired()) {
            if(!playingTiredPS) {
                tiredPS.Play();
                playingTiredPS = true;
            }
            runProgressionBar.color = tiredColor;
        }
        else {
            runProgressionBar.color = runColor;
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
            runProgressionBar.color = extremeColor;
        } else {
            extremelyTiredPS.Stop();
            playingExtremelyTiredPS = false;
        }

        
    }

}
