using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteWolfVisual : MonoBehaviour
{

    [SerializeField] WhiteWolfAI wolfAI;
    [SerializeField] GameObject wolfFleeIndicator;

    private void Start() {
        wolfFleeIndicator.SetActive(false);
    }

    private void Update() {
        if (wolfAI.GetState() == WhiteWolfAI.State.Flee) {
            wolfFleeIndicator.SetActive(true);
        }
        else {
            wolfFleeIndicator.SetActive(false);
        }
    }
}
