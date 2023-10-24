using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SheepSO : ScriptableObject
{
    public float triggerAggregateDistance;

    public float roamPauseMaxTime;
    public float roamPauseMinTime;

    public float moveSpeed;
    public float aggregateSpeed;
    public float roamSpeed;
    public float injuredSpeedFactor;
}
