using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SetAIAnimatorParameters))]

public class Sheep : MonoBehaviour
{

    private Sheep[] sheepArray;
    [SerializeField] LayerMask sheepLayerMask;
    public event EventHandler<OnSheepEnterScoreZoneEventArgs> OnSheepEnterScoreZone;
    private bool hasEnteredScoreZone;

    [SerializeField] int sheepMinimumNumber = 3;
    [SerializeField] float sphereRadius = 3f;
    public class OnSheepEnterScoreZoneEventArgs : EventArgs {
        public Transform[] scoreZoneAggregatePointArray;
    }

    protected virtual void Awake() {
        sheepArray = SheepObjectPool.Instance.GetSheepArray();
    }

    public Transform GetClosestSheepWithEnoughSheepSurrounding() {

        Transform closestSheepWithEnoughSheepSurrounding = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        if (sheepArray.Length == 1) {
            return null;
        }

        foreach (Sheep potentialSheep in sheepArray) {

            // Distance to sheep
            Vector3 directionToSheep = potentialSheep.transform.position - currentPosition;
            float dSqrToSheep = directionToSheep.sqrMagnitude;

            // Sheep surroundings
            int sheepNumberWithinTargetSheepRadius = Physics2D.OverlapCircleAll(potentialSheep.transform.position, sphereRadius, sheepLayerMask).Length;
            Debug.DrawLine(potentialSheep.transform.position, potentialSheep.transform.position + new Vector3(sphereRadius, 0));

            if (dSqrToSheep < closestDistanceSqr & dSqrToSheep != 0 & sheepNumberWithinTargetSheepRadius >= sheepMinimumNumber) {
                closestDistanceSqr = dSqrToSheep;
                closestSheepWithEnoughSheepSurrounding = potentialSheep.transform;
            }
        }
        return closestSheepWithEnoughSheepSurrounding;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject collisionGameObject = collision.gameObject;

        if (collisionGameObject.TryGetComponent<ScoreZone>(out ScoreZone scoreZone)) {

            if (!hasEnteredScoreZone) {

                // Select random aggregate point in score zone and pass it in event
                Transform[] aggregatePointArray = scoreZone.GetAggregatePointArray();
                OnSheepEnterScoreZone?.Invoke(this, new OnSheepEnterScoreZoneEventArgs {
                    scoreZoneAggregatePointArray = aggregatePointArray
                });
            }

            hasEnteredScoreZone = true;
        }
    }
}
