using Febucci.UI;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] private TypewriterByCharacter logoText;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator logoAnimator;

    [SerializeField] private float timeToStartShowLogo;
    [SerializeField] private float timeToStartShowText;
    [SerializeField] private float timeToStartTiltLogo;


    [SerializeField] private float slowTypingSpeed;
    [SerializeField] private float originalTypingSpeed;

    [SerializeField] private MMFeedbacks screenshake;

    private float splashScreenTimer = 5f;
    private int caseNumber;
    private string logoTextWithOptions = "{fade d=1}<b>Un<color=#2F2A26>{/fade}{fade d=3}paws </color></b>{/fade}{fade d=1}Games{/fade}";

    bool splashScreenOver;
    bool hasScreenShaked = false;
    bool hasPlayedImpact = false;

    private void Start() {
        caseNumber = 0;
        text.text = logoTextWithOptions;

        StartCoroutine(ShowLogo(timeToStartShowLogo));
        StartCoroutine(ShowText(timeToStartShowText));
        StartCoroutine(TiltLogo(timeToStartTiltLogo));
    }

    private void Update() {
        splashScreenTimer -= Time.deltaTime;
        if (splashScreenTimer < 0 & !splashScreenOver) {
            splashScreenOver = true;
            PostSplashScreenSceneLoader.Instance.LoadNextScene(0f);
        }

        if (Input.anyKeyDown && SplashScreen.isFinished) {
            PostSplashScreenSceneLoader.Instance.LoadNextScene(0f);
        }

        if (hasPlayedImpact == false && logoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.748) {
            SoundManager.instance.PlayImpact();
            hasPlayedImpact = true;
        }

        if (hasScreenShaked == false && logoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95) {
            hasScreenShaked = true;
            screenshake.PlayFeedbacks();
        }
    }

    private IEnumerator ShowLogo(float time) {
        yield return new WaitForSeconds(time);
        logoAnimator.SetTrigger("Appear");
    }

    private IEnumerator ShowText(float pauseDuration) {
        yield return new WaitForSeconds(pauseDuration);
        logoText.SetTypewriterSpeed(originalTypingSpeed);
        logoText.StartShowingText();
    }

    public IEnumerator TiltLogo(float pauseDuration) {
        yield return new WaitForSeconds(timeToStartTiltLogo);
            logoAnimator.SetTrigger("Tilt");
    }



    public void ChangeTextSpeed() {
        caseNumber++;

        if (caseNumber == 2) {
            logoText.SetTypewriterSpeed(slowTypingSpeed);
        }

        if (caseNumber == 7) {
            logoText.SetTypewriterSpeed(originalTypingSpeed);
        }

    }

}
