using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(SetAIAnimatorParameters))]

public class Sheep : MonoBehaviour
{

    private Sheep[] sheepArray;
    public event EventHandler<OnSheepEnterScoreZoneEventArgs> OnSheepEnterScoreZone;

    public class OnSheepEnterScoreZoneEventArgs : EventArgs {
        public Transform[] scoreZoneAggregatePointArray;
    }

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

    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject collisionGameObject = collision.gameObject;

        if (collisionGameObject.TryGetComponent<ScoreZone>(out ScoreZone scoreZone)) {

            Transform[] aggregatePointArray = scoreZone.GetAggregatePointArray();
            OnSheepEnterScoreZone?.Invoke(this, new OnSheepEnterScoreZoneEventArgs {
                scoreZoneAggregatePointArray = aggregatePointArray
            });
        }
    }
}
