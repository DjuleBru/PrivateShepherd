using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class OpeningManager : MonoBehaviour
{

    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;

    private float introTime = 6.5f;
    private float introTimer;
    private UnityEngine.AsyncOperation asyncOperation;
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private RawImage foregroundVideo;

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RenderTexture splashScreenLandscape;
    [SerializeField] private RenderTexture splashScreenPortrait;

    private void Start() {
        introTimer = introTime;

        AssignTextureByAspectRatio();
        StartCoroutine(HideImage());
        StartCoroutine(PreLoadScene("SampleScene"));
    }

    private void Update() {
        introTimer -= Time.deltaTime;
       
        if ((introTimer < 0 | Input.anyKeyDown) && SplashScreen.isFinished) {
            StartCoroutine(LoadScene());
        }
    }

    private void AssignTextureByAspectRatio() {
        if (Camera.main.aspect >= 1.7) {
           videoPlayer.targetTexture = splashScreenLandscape;
           foregroundVideo.texture = splashScreenLandscape;
           Debug.Log("16:9");
        }
        else if (Camera.main.aspect >= 1.5) {
            Debug.Log("3:2");
        }
        else {
            videoPlayer.targetTexture = splashScreenPortrait;
            foregroundVideo.texture = splashScreenPortrait;
            Debug.Log("4:3");
        }
    }

    private IEnumerator HideImage() {
        yield return new WaitForSeconds(.5f);
        backgroundImage.gameObject.SetActive(false);
    }
     private IEnumerator PreLoadScene(string levelString) {
        asyncOperation = SceneManager.LoadSceneAsync(levelString);
        asyncOperation.allowSceneActivation = false;
        yield return null;
    }

    public IEnumerator LoadScene() {
        // PLay Animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        asyncOperation.allowSceneActivation = true;
    }

}
