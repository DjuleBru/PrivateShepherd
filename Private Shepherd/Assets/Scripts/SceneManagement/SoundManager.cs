using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
  
    public static SoundManager instance;

    [SerializeField] private AudioClip wooshSFX;
    [SerializeField] private AudioClip woosh2SFX;
    [SerializeField] private AudioClip impactSFX;
    [SerializeField] private AudioClip[] lettersTypingSFX;

    private AudioSource audioSource;

    private void Awake() {
        instance = this;
    }

    public void Start() {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayWoosh());
    }
    public IEnumerator PlayWoosh() {
        yield return new WaitForSeconds(.1f);
        audioSource.PlayOneShot(wooshSFX);
    }

    public void PlayImpact() {
        audioSource.PlayOneShot(impactSFX, .2f);
    }

    public void PlayWoosh2() {
        audioSource.PlayOneShot(woosh2SFX, .3f);
    }

}
