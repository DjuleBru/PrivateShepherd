using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeTarget : MonoBehaviour
{
    [SerializeField] private float fleeTargetTriggerDistance;
    [SerializeField] private float fleeTargetStopDistance;

    public float GetFleeTargetTriggerDistance() {
        return fleeTargetTriggerDistance;
    }
    public float GetFleeTargetStopDistance() {
        return fleeTargetStopDistance;
    }
}
