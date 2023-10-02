using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBarkVisual : MonoBehaviour
{

    [SerializeField] private ParticleSystem dogBarkPS;
    [SerializeField] private GameObject barkAOEVisual;
    private SpriteRenderer barkAOESprite;

    private bool barkUnlocked = true;
    private bool barkFading;
    private float barkCoolDownTimer = 5f;
    private float barkCoolDown = .5f;

    private void Start() {
        barkAOEVisual.SetActive(false);
        barkAOESprite = barkAOEVisual.GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (!barkUnlocked) {
            return;
        }

        barkCoolDownTimer -= Time.deltaTime;

        if (Input.GetKey(KeyCode.E)) {
            // Player is preparing bark
            // Show AOE with low alpha

            if (barkCoolDownTimer < 0f) {
                // Bark is ready
                Color tmpColor = Color.green;
                tmpColor.a = .2f;
                barkAOESprite.color = tmpColor;
            }
            else {
                // Bark is not ready
                Color tmpColor = Color.white;
                tmpColor.a = .2f;
                barkAOESprite.color = tmpColor;
            }
        }
        else {
            // Player is not pressing any key
            if (!barkFading) {
                Color tmpColor = Color.white;
                tmpColor.a = 0f;
                barkAOESprite.color = tmpColor;
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && barkCoolDownTimer < 0) {
            StartCoroutine(Bark());
        }
    }

    private IEnumerator Bark() {
        dogBarkPS.Play();

        float barkAOEFadeSpeed = .1f;

        for (float alpha = 1f; alpha >= -.1; alpha -= .1f) {
            barkFading = true;

            Color spriteColor = barkAOESprite.color;
            spriteColor.a = alpha;
            barkAOESprite.color = spriteColor;
            yield return new WaitForSeconds(barkAOEFadeSpeed);
        }
        barkFading = false;
    }

}
