using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfVisual : MonoBehaviour
{
    [SerializeField] WolfAI wolfAI;
    [SerializeField] GameObject wolfFleeIndicator;

    private void Start() {
        wolfFleeIndicator.SetActive(false);
    }

    private void Update() {
        if (wolfAI.GetState() == WolfAI.State.Flee) {
            wolfFleeIndicator.SetActive(true);
        }
        else {
            wolfFleeIndicator.SetActive(false);
        }
    }
}
