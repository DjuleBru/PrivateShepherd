using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class AIMovement : MonoBehaviour
{
    #region PATH COMPONENTS

    protected Seeker seeker;
    protected Path path;
    protected Vector3 velocity;
    [SerializeField] protected Rigidbody2D rb;

    #endregion

    #region PATH PARAMETERS

    // Length of the path
    protected int theGScoreToStopAt = 6000;

    protected float nextWaypointDistance = 1.5f;
    [SerializeField] float nextWaypointDistancePersonalized;
    [SerializeField] protected float roamPointRadius = 4f;

    protected int currentWaypoint = 0;
    protected bool reachedEndOfPath;

    protected float pathCalculationRate = .2f;
    protected float pathCalculationTimer = 0;
    #endregion

    protected Vector3 moveDirNormalized;
    protected Vector2 moveDir2DNormalized;
    protected float moveSpeed;

    protected bool cutSceneInProgress;

    protected virtual void Start() {

        seeker = GetComponent<Seeker>();
        //Initialise path
        CalculatePath(transform.position);

        pathCalculationTimer = 0;

        if (LevelManager.Instance != null) {
            LevelManager.Instance.OnCutSceneEnter += LevelManager_OnCutSceneEnter;
            LevelManager.Instance.OnCutSceneExit += LevelManager_OnCutSceneExit;
        }
    }

    protected virtual void LateUpdate() {

        if (nextWaypointDistancePersonalized != 0) {
            nextWaypointDistance = nextWaypointDistancePersonalized;
        }
        pathCalculationTimer -= Time.deltaTime;
        Move(velocity);
    }

    protected void FollowPath(Path path) {
        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true) {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

            if (distanceToWaypoint < nextWaypointDistance) {

                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                }
                else {
                    reachedEndOfPath = true;
                    break;
                }
            }
            else {
                break;
            }
        }
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

        moveDirNormalized = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        moveDir2DNormalized = new Vector2 (moveDirNormalized.x, moveDirNormalized.y);
        velocity = moveDir2DNormalized * moveSpeed * speedFactor;
    }

    protected virtual void Move(Vector3 velocity) {
        if (!reachedEndOfPath) {
            rb.velocity = velocity * Time.fixedDeltaTime;
        }
        else {
            rb.velocity = Vector3.zero;
        }
    }

    protected void CalculatePath(Vector3 destinationPoint) {
        if (pathCalculationTimer <= 0) {
            seeker.StartPath(transform.position, destinationPoint, PathComplete);
            pathCalculationTimer = pathCalculationRate;
        }
    }

    protected void CalculateFleePath(Vector3 fleePosition) {
        if (pathCalculationTimer <= 0) {
            // Create a path object
            FleePath path = FleePath.Construct(transform.position, fleePosition, theGScoreToStopAt);

            // This is how strongly it will try to flee, if you set it to 0 it will behave like a RandomPath
            path.aimStrength = 1f;

            // Determines the variation in path length that is allowed
            path.spread = 1000;

            seeker.StartPath(path, FleePathComplete);

            pathCalculationTimer = pathCalculationRate;
        }
    }

    protected void FleePathComplete(Path p) {
        path = p;
        currentWaypoint = 0;
    }

    protected void PathComplete(Path p) {
        path = p;
        currentWaypoint = 0;
    }

    protected Vector3 PickRandomPoint(Vector3 initialPoint) {
        var point = UnityEngine.Random.insideUnitSphere * roamPointRadius;

        point.z = 0;
        point += initialPoint;
        return point;
    }

    public Vector3 GetMoveDirNormalized() {
        return moveDirNormalized;
    }

    public bool GetReachedEndOfPath() {
        return reachedEndOfPath;
    }

    private void LevelManager_OnCutSceneExit(object sender, EventArgs e) {
        cutSceneInProgress = false;
    }

    private void LevelManager_OnCutSceneEnter(object sender, EventArgs e) {
        cutSceneInProgress = true;
    }
}
