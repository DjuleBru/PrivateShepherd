using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteSheep : Sheep
{
    [SerializeField] private SheepMovement sheepMovement;
    [SerializeField] private Transform[] fleeTargetArray;

    private void Start() {

        sheepMovement.AddFleeTarget(Player.Instance.transform);
        foreach (Transform fleeTarget in fleeTargetArray) {
            sheepMovement.AddFleeTarget(fleeTarget);
        }
    }

}
