using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WolfSO : ScriptableObject
{
    public float roamPauseMaxTime;
    public float roamPauseMinTime;

    public float agressiveFleeTime;
    public float roamToAgressiveTime;
    public float agressiveTargetSheepRate;

    public int maxSheepInHerdToAttack;

    public float agressiveSpeed;
    public float fleeSpeed;
    public float roamSpeed;
    public float attackSpeed;

    public float agressiveMinDistanceToSheep;
    public float agressiveMaxDistanceToSheep;

    public float wolfTriggerFleeDistanceMultiplier;
    public float closestFleeTargetTriggerFleeDistance;
    public float closestFleeTargetStopDistance;
    public float closestFleeTargetDistanceToEatSheep;

}
