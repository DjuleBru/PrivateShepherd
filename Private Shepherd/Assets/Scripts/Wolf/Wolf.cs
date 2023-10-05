using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    [SerializeField] FleeTarget[] fleeTargetArray;
    [SerializeField] WolfMovement wolfMovement;

    private void Start() {
        foreach(FleeTarget fleeTarget in fleeTargetArray) {
            wolfMovement.AddFleeTarget(fleeTarget);
        }
    }
}
