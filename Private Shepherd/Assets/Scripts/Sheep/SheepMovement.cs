using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    [SerializeField] private Sheep sheep;
    [SerializeField] private SheepSO sheepSO;
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private Transform closestSheepTransform;
    private Vector3 moveDir;

    #region FLEE TARGET PARAMETERS
    private FleeTarget closestFleeTarget;
    float closestFleeTargetDistance;
    float fleeTargetDistance;
    float fleeTargetStopDistance;
    float fleeTargetTriggerFleeDistance;
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
    #endregion

    public enum State {
        Flee,
        Aggregate,
        Roam,
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
                moveSpeed = fleeSpeed;

                FindClosestFleeTargetWithinTriggerRadius();

                closestFleeTargetDistance = Vector3.Distance(closestFleeTarget.transform.position, transform.position);
                closestFleeTargetStopDistance = closestFleeTarget.GetFleeTargetStopDistance();

                if (closestFleeTargetDistance > closestFleeTargetStopDistance) {
                    // Closest target is out of flee trigger radius
                    state = State.Aggregate;
                    return;

                } else {
                    // Target is within flee trigger radius
                    if (pathCalculationTimer <= 0f) {
                        CalculateFleePath(closestFleeTarget.transform.position);
                        pathCalculationTimer = pathCalculationRate;
                    }
                }

                FollowPath(path);

            break;
            case State.Aggregate:
                moveSpeed = aggregateSpeed;
                closestSheepTransform = sheep.GetClosestSheep();

                if (closestSheepTransform == null) {
                    // There is no other sheep to aggregate to
                    state = State.Roam;
                    return;
                }

                foreach (FleeTarget fleeTarget in fleeTargetList) {
                    fleeTargetDistance = Vector3.Distance(fleeTarget.transform.position, transform.position);
                    fleeTargetTriggerFleeDistance = fleeTarget.GetFleeTargetTriggerDistance();

                    if (fleeTargetDistance <= fleeTargetTriggerFleeDistance) {
                        // Target is within flee trigger radius
                        state = State.Flee;
                        return;
                    }
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

                foreach (FleeTarget fleeTarget in fleeTargetList) {

                    fleeTargetDistance = Vector3.Distance(fleeTarget.transform.position, transform.position);
                    fleeTargetTriggerFleeDistance = fleeTarget.GetFleeTargetTriggerDistance();

                    if (fleeTargetDistance <= fleeTargetTriggerFleeDistance) {
                        // Target is within flee trigger radius
                        state = State.Flee;
                        return;
                    }
                }

                closestSheepTransform = sheep.GetClosestSheep();
                if (closestSheepTransform != null) {
                    // There is another sheep to aggregate
                    if (Vector3.Distance(closestSheepTransform.position, transform.position) > triggerAggregateDistance) {
                        state = State.Aggregate;
                    }
                }
                
                if (roamPauseTimer <= 0) {
                    Vector3 roamDestination = PickRandomPoint();
                    CalculatePath(roamDestination);
                    roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                }

                FollowPath(path);
                break;
        }
    }

    private void FindClosestFleeTargetWithinTriggerRadius() {

        closestFleeTargetDistance = float.MaxValue;

        foreach (FleeTarget fleeTarget in fleeTargetList) {
            fleeTargetDistance = Vector3.Distance(fleeTarget.transform.position, transform.position);

            fleeTargetTriggerFleeDistance = fleeTarget.GetFleeTargetTriggerDistance();
            fleeTargetStopDistance = fleeTarget.GetFleeTargetStopDistance();

            if (fleeTargetDistance < closestFleeTargetDistance & fleeTargetStopDistance > fleeTargetDistance) {
                // The flee target is the closest one AND within trigger radius

                closestFleeTargetDistance = fleeTargetDistance;
                closestFleeTarget = fleeTarget;
                closestFleeTargetStopDistance = fleeTargetStopDistance;
            }
        }
    }

    private void PlayerBark_OnPlayerBark(object sender, System.EventArgs e) {
        StartCoroutine(SetFleeSpeed(sheepSO.barkFleeSpeed));
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

    private Vector3 PickRandomPoint() {
        var point = UnityEngine.Random.insideUnitSphere * roamPointRadius;

        point.z = 0;
        point += transform.position;
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
