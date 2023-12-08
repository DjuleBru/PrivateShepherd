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
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneByStringCoroutine(sceneName));
    }

    public void LoadCurrentScene() {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneByIndexCoroutine(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadMainMenu() {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneByStringCoroutine("MainMenu"));
    }

    public void LoadWorldMap() {
        Time.timeScale = 1f;
        Debug.Log(ES3.Load("Tutorial_completed", false));
        if (!ES3.Load("Tutorial_completed", false)) {
            StartCoroutine(LoadSceneByStringCoroutine("Tutorial"));
        } else {
            StartCoroutine(LoadSceneByStringCoroutine("WorldMap"));
        }
    }

    public IEnumerator LoadSceneByStringCoroutine(string sceneName) {
        Time.timeScale = 1f;

        yield return new WaitForSeconds(loadSceneDelay);

        // PLay Animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator LoadSceneByIndexCoroutine(int sceneIndex) {
        Time.timeScale = 1f;

        yield return new WaitForSeconds(loadSceneDelay);

        // PLay Animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame() {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
