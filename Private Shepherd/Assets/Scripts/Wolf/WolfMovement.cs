using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfMovement : AIMovement
{

    [SerializeField] private WolfSO wolfSO;
    private List<Sheep> sheepsinLevel;
    private State state;
    public enum State {
        Flee,
        Roam,
        Attack,
    }

    #region FLEE TARGET PARAMETERS
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private FleeTarget closestFleeTarget;
    float closestFleeTargetDistance = float.MaxValue;
    float closestFleeTargetTriggerFleeDistance;
    float closestFleeTargetStopDistance;
    #endregion

    #region WOLF PARAMETERS
    private float roamPauseMaxTime;
    private float roamPauseMinTime;
    private float roamPauseTimer;

    private float fleeSpeed;
    private float roamSpeed;
    #endregion

    private void Awake() {
        state = State.Roam;

        roamPauseMaxTime = wolfSO.roamPauseMaxTime;
        roamPauseMinTime = wolfSO.roamPauseMinTime;
        fleeSpeed = wolfSO.fleeSpeed;
        roamSpeed = wolfSO.roamSpeed;
    }

    protected override void Start() {
        seeker = GetComponent<Seeker>();

        //Initialise path
        CalculatePath(transform.position);
        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
    }

    private void Update() {
        pathCalculationTimer -= Time.deltaTime;

        if (path == null) {
            return;
        }
        switch (state) {
            case State.Flee:

                break;

            case State.Roam:
                moveSpeed = roamSpeed;
                roamPauseTimer -= Time.deltaTime;

                if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
                    // Target is within flee trigger radius
                    state = State.Flee;
                    return;
                }
                if (roamPauseTimer <= 0) {
                    Vector3 roamDestination = PickRandomPoint(transform.position);
                    CalculatePath(roamDestination);
                    roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                }

                FollowPath(path);
                break;
            case State.Attack:
                break;
        }
    }

}
