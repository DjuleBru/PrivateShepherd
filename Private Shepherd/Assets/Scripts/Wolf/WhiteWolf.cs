using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteWolf : MonoBehaviour
{
    [SerializeField] FleeTarget[] fleeTargetArray;
    [SerializeField] WhiteWolfAI wolfAI;

    private void Start() {
        foreach (FleeTarget fleeTarget in fleeTargetArray) {
            wolfAI.AddFleeTarget(fleeTarget);
        }
    }
}
