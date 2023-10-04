using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sheep;

public class SheepMovement : MonoBehaviour
{
    [SerializeField] private Sheep sheep;
    [SerializeField] private SheepSO sheepSO;
    private Transform closestSheepTransform;
    private Transform targetScoreAggregatePoint; 
    private Vector3 moveDir;

    public event EventHandler OnSheepFlee;

    #region FLEE TARGET PARAMETERS
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private FleeTarget closestFleeTarget;
    float closestFleeTargetDistance = float.MaxValue;
    float closestFleeTargetTriggerFleeDistance;
    float closestFleeTargetStopDistance;
    #endregion

    #region PATH COMPONENTS

    private Seeker seeker;
    private Path path;

    #endregion

    #region PATH PARAMETERS

    // Length of the path
    private int theGScoreToStopAt = 6000;

    private float nextWaypointDistance = 1.5f;
    private float roamPointRadius = 4f;

    private int currentWaypoint = 0;
    private bool reachedEndOfPath;

    private float pathCalculationRate = .2f;
    private float pathCalculationTimer;
    #endregion

    #region SHEEP PARAMETERS
    private float triggerAggregateDistance;

    private float roamPauseMaxTime;
    private float roamPauseMinTime;
    private float roamPauseTimer;

    private float moveSpeed;
    private float fleeSpeed;
    private float aggregateSpeed;
    private float roamSpeed;

    private bool penned;
    #endregion

    public enum State {
        Flee,
        Aggregate,
        Roam,
        InScoreZone,
    }

    private State state;

    private void Awake() {
        triggerAggregateDistance = sheepSO.triggerAggregateDistance;
        roamPauseMaxTime = sheepSO.roamPauseMaxTime;
        roamPauseMinTime = sheepSO.roamPauseMinTime;
        moveSpeed = sheepSO.moveSpeed;
        fleeSpeed = sheepSO.fleeSpeed;
        aggregateSpeed = sheepSO.aggregateSpeed;
        roamSpeed = sheepSO.roamSpeed;

        state = State.Aggregate;
        pathCalculationTimer = pathCalculationRate;
    }

    private void Start() {
        seeker = GetComponent<Seeker>();
        PlayerBark.Instance.OnPlayerBark += PlayerBark_OnPlayerBark;
        sheep.OnSheepEnterScoreZone += Sheep_OnSheepEnterScoreZone;

        //Initialise path
        CalculatePath(transform.position);
        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
    }

    private void Update() {
        pathCalculationTimer -= Time.deltaTime;

        if (path == null) {
            return;
        }

        closestFleeTarget = FindClosestFleeTarget();
        UpdateClosestFleeTargetParameters();

        switch (state) {
            case State.Flee:
                moveSpeed = fleeSpeed * closestFleeTarget.GetFleeTargetSpeedMultiplier();

                if (closestFleeTargetDistance > closestFleeTargetStopDistance) {
                    // Closest target is out of flee stop radius
                    state = State.Aggregate;
                    return;

                } else {
                    // Target is within flee stop radius
                    if (pathCalculationTimer <= 0f) {
                        CalculateFleePath(closestFleeTarget.transform.position);
                        pathCalculationTimer = pathCalculationRate;
                    }
                }

                FollowPath(path);

            break;
            case State.Aggregate:
                moveSpeed = aggregateSpeed;
                closestSheepTransform = sheep.GetClosestSheepWithEnoughSheepSurrounding();

                if (closestSheepTransform == null) {
                    // There is no other sheep to aggregate to
                    state = State.Roam;
                    return;
                }

                if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
                    // Target is within flee trigger radius
                    state = State.Flee;
                    OnSheepFlee?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (Vector3.Distance(closestSheepTransform.position, transform.position) < triggerAggregateDistance) {
                    reachedEndOfPath = true;
                    state = State.Roam;
                    roamPauseTimer = 0;
                }

                if (pathCalculationTimer <= 0f) {
                    CalculatePath(closestSheepTransform.position);
                    pathCalculationTimer = pathCalculationRate;
                }

                FollowPath(path);

            break;
            case State.Roam:
                moveSpeed = roamSpeed;
                roamPauseTimer -= Time.deltaTime;

                if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
                    // Target is within flee trigger radius
                    state = State.Flee;
                    OnSheepFlee?.Invoke(this, EventArgs.Empty);
                    return;
                }

                closestSheepTransform = sheep.GetClosestSheepWithEnoughSheepSurrounding();
                if (closestSheepTransform != null) {
                    // There is another sheep to aggregate
                    if (Vector3.Distance(closestSheepTransform.position, transform.position) > triggerAggregateDistance) {
                        state = State.Aggregate;
                    }
                }
                
                if (roamPauseTimer <= 0) {
                    Vector3 roamDestination = PickRandomPoint(transform.position);
                    CalculatePath(roamDestination);
                    roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                }

                FollowPath(path);
                break;

            case State.InScoreZone:
                moveSpeed = roamSpeed;
                roamPauseTimer -= Time.deltaTime;

                float pennedDistanceTreshold = 1f;
                if (Vector3.Distance(targetScoreAggregatePoint.position, transform.position) < pennedDistanceTreshold) {
                    // Check if sheep is close enough to aggregatePoint
                    penned = true;
                }

                if(!penned) {
                    CalculatePath(targetScoreAggregatePoint.position);
                } else {
                    if (roamPauseTimer <= 0) {
                        Vector3 roamDestination = PickRandomPoint(targetScoreAggregatePoint.position);
                        CalculatePath(roamDestination);
                        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                    }
                }

                FollowPath(path);
                break;
        }
    }

    private FleeTarget FindClosestFleeTarget() {

        float fleeTargetDistance;

        foreach (FleeTarget fleeTarget in fleeTargetList) {
            fleeTargetDistance = Vector3.Distance(fleeTarget.transform.position, transform.position);

            if (fleeTargetDistance < closestFleeTargetDistance) {
                // The flee target is the closest one

                closestFleeTargetDistance = fleeTargetDistance;
                closestFleeTarget = fleeTarget;
            }
        }

        return closestFleeTarget;
    }

    private void UpdateClosestFleeTargetParameters() {
        closestFleeTargetDistance = Vector3.Distance(closestFleeTarget.transform.position, transform.position);
        closestFleeTargetTriggerFleeDistance = closestFleeTarget.GetFleeTargetTriggerDistance();
        closestFleeTargetStopDistance = closestFleeTarget.GetFleeTargetStopDistance();
    }


    private void PlayerBark_OnPlayerBark(object sender, System.EventArgs e) {
        StartCoroutine(SetFleeSpeed(sheepSO.barkFleeSpeed));
    }

    private void Sheep_OnSheepEnterScoreZone(object sender, OnSheepEnterScoreZoneEventArgs e) {
        state = State.InScoreZone;
        Transform[] scoreZoneAggregatePointArray = e.scoreZoneAggregatePointArray;

        // pick a random aggregate point in scorezone
        targetScoreAggregatePoint = scoreZoneAggregatePointArray[UnityEngine.Random.Range(0, scoreZoneAggregatePointArray.Length)];
    }

    private void FollowPath(Path path) {
        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true) {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

            if (distanceToWaypoint < nextWaypointDistance) {

                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    reachedEndOfPath = true;
                    break;
                }
            } else {
                break;
            }
        }
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

        moveDir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 velocity = moveDir * moveSpeed * speedFactor;

        if (!reachedEndOfPath) {
            transform.position += velocity * Time.deltaTime;
        }
    }

    private void CalculatePath(Vector3 destinationPoint) {
        seeker.StartPath(transform.position, destinationPoint, PathComplete);
    }

    private void CalculateFleePath(Vector3 fleePosition) {
        // Create a path object
        FleePath path = FleePath.Construct(transform.position, fleePosition, theGScoreToStopAt);

        // This is how strongly it will try to flee, if you set it to 0 it will behave like a RandomPath
        path.aimStrength = 1f;

        // Determines the variation in path length that is allowed
        path.spread = 1000;

        seeker.StartPath(path, FleePathComplete);
    }

    private void FleePathComplete(Path p) {
        path = p;
        currentWaypoint = 0;
    }

    private void PathComplete(Path p) {
        path = p;
        currentWaypoint = 0;
    }

    private Vector3 PickRandomPoint(Vector3 initialPoint) {
        var point = UnityEngine.Random.insideUnitSphere * roamPointRadius;

        point.z = 0;
        point += initialPoint;
        return point;
    }
    public Vector3 GetMoveDir() {
        return moveDir;
    }

    public bool GetReachedEndOfPath() {
        return reachedEndOfPath;
    }

    public void AddFleeTarget(FleeTarget fleeTarget) {
        fleeTargetList.Add(fleeTarget);
    }

    public IEnumerator SetFleeSpeed(float newFleeSpeed) {
        fleeSpeed = newFleeSpeed;

        yield return new WaitForSeconds(1f);

        fleeSpeed = sheepSO.fleeSpeed;
    }

}
