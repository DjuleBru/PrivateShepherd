using Pathfinding.Ionic.Zip;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FleeTarget : MonoBehaviour
{
    [SerializeField] private float fleeTargetTriggerDistance;
    [SerializeField] private float fleeTargetStopDistance;
    [SerializeField] private float fleeTargetSpeedMultiplier;

    //void OnDrawGizmos() {
    //    // Draw a yellow sphere at the transform's position
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(transform.position, fleeTargetTriggerDistance);
    //}

    public float GetFleeTargetTriggerDistance() {
        return fleeTargetTriggerDistance;
    }
    public float GetFleeTargetStopDistance() {
        return fleeTargetStopDistance;
    }

    public float GetFleeTargetSpeedMultiplier() {
        return fleeTargetSpeedMultiplier;
    }

    public void SetFleeTargetTriggerDistance(float fleeTargetTriggerDistance) {
        this.fleeTargetTriggerDistance = fleeTargetTriggerDistance;
    }

    public void SetFleeTargetStopDistance(float fleeTargetStopDistance) {
        this.fleeTargetStopDistance = fleeTargetStopDistance;
    }

    public void SetFleeTargetSpeedMultiplier(float fleeTargetSpeedMultiplier) {
        this.fleeTargetSpeedMultiplier *= fleeTargetSpeedMultiplier;
    }

}
