using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDogIconAnimator : MonoBehaviour
{

    [SerializeField] ButtonScript startGameButtonScript;
    [SerializeField] Animator dogIconAnimator;

    private void Start() {
        startGameButtonScript = startGameButtonScript.GetComponent<ButtonScript>();
        startGameButtonScript.OnButtonDeSelected += StartGameButtonScript_OnButtonDeSelected;
        startGameButtonScript.OnButtonSelected += StartGameButtonScript_OnButtonSelected;
    }

    private void StartGameButtonScript_OnButtonSelected(object sender, System.EventArgs e) {
        dogIconAnimator.SetBool("Walk", true);
    }

    private void StartGameButtonScript_OnButtonDeSelected(object sender, System.EventArgs e) {
        dogIconAnimator.SetBool("Walk", false);
    }
}
