using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTouchButton : MonoBehaviour
{
    [SerializeField] Button barkButton;
    [SerializeField] GameObject barkButtonGO;
    [SerializeField] GameObject sneakButtonGO;
    [SerializeField] GameObject growlButtonGO;
    [SerializeField] GameObject runButtonGO;
    [SerializeField] GameObject touchJoystickGO;

    bool androidPort;

    void Start()
    {
        androidPort = Player.Instance.GetAndroidPort();
        if (!androidPort) {
            barkButtonGO.SetActive(false);
            sneakButtonGO.SetActive(false); 
            growlButtonGO.SetActive(false);
            runButtonGO.SetActive(false);
            touchJoystickGO.SetActive(false);
            return;
        }

        barkButton.onClick.AddListener(BarkButtonClick);
    }


    void BarkButtonClick() {
        PlayerBark.Instance.BarkTouch();
    }

    public void GrowlButtonDown() {
        PlayerGrowl.Instance.GrowlTouchPerformed();
    }

    public void GrowlButtonUp() {
        PlayerGrowl.Instance.GrowlTouchReleased();
    }

    public void SneakButtonDown() {
        PlayerSneak.Instance.SneakTouchPerformed();
    }

    public void SneakButtonUp() {
        PlayerSneak.Instance.SneakTouchReleased();
    }

    public void RunButtonUp() {
        PlayerRun.Instance.RunTouchReleased();
    }

    public void RunButtonDown() {
        PlayerRun.Instance.RunTouchPerformed();
    }
}
