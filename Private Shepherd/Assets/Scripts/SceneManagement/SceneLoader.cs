using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;

    [SerializeField] private float loadSceneDelay;

    private void Start() {
        Instance = this;
    }

    public void LoadScene(string sceneName) {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public void LoadMainMenu() {
        StartCoroutine(LoadSceneCoroutine("MainMenu"));
    }

    public void LoadWorldMap() {
        StartCoroutine(LoadSceneCoroutine("WorldMap"));
    }

    public IEnumerator LoadSceneCoroutine(string sceneName) {

        yield return new WaitForSeconds(loadSceneDelay);

        // PLay Animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        SceneManager.LoadScene(sceneName);
    }
}
