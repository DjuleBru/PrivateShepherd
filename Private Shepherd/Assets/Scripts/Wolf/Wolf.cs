using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    [SerializeField] FleeTarget[] fleeTargetArray;
    [SerializeField] WolfAI wolfAI;

    private void Start() {
        foreach(FleeTarget fleeTarget in fleeTargetArray) {
            wolfAI.AddFleeTarget(fleeTarget);
        }
    }
}
