using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetAIAnimatorParameters))]

public class Sheep : MonoBehaviour
{

    private Sheep[] sheepArray;

    protected virtual void Awake() {
        sheepArray = SheepObjectPool.Instance.GetSheepArray();
    }

    public Transform GetClosestSheep() {

        Transform closestSheep = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        if (sheepArray.Length == 1) {
            return null;
        }

        foreach (Sheep potentialSheep in sheepArray) {

            Vector3 directionToSheep = potentialSheep.transform.position - currentPosition;
            float dSqrToSheep = directionToSheep.sqrMagnitude;

            if (dSqrToSheep < closestDistanceSqr && dSqrToSheep != 0) {
                closestDistanceSqr = dSqrToSheep;
                closestSheep = potentialSheep.transform;
            }
        }
        return closestSheep;
    }
}
