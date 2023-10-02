using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;

    private UnityEngine.AsyncOperation asyncOperation;

    private void Start() {
        Instance = this;
        StartCoroutine(PreLoadScene("SampleScene"));
    }

    private IEnumerator PreLoadScene(string levelString) {
        asyncOperation = SceneManager.LoadSceneAsync(levelString);
        asyncOperation.allowSceneActivation = false;
        yield return null;
    }

    public void LoadNextScene(float loadSceneDelay) {
        StartCoroutine(LoadScene(loadSceneDelay));
    }

    public IEnumerator LoadScene(float loadSceneDelay) {

        yield return new WaitForSeconds(loadSceneDelay);

        // PLay Animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        asyncOperation.allowSceneActivation = true;
    }
}
