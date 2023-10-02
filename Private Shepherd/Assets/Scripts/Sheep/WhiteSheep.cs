using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteSheep : Sheep
{
    [SerializeField] private SheepMovement sheepMovement;
    [SerializeField] private FleeTarget[] fleeTargetArray;

    private void Start() {

        foreach (FleeTarget fleeTarget in fleeTargetArray) {
            sheepMovement.AddFleeTarget(fleeTarget);
        }
    }

}
