using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class AIMovement : MonoBehaviour
{
    #region PATH COMPONENTS

    protected Seeker seeker;
    protected Path path;

    #endregion

    #region PATH PARAMETERS

    // Length of the path
    protected int theGScoreToStopAt = 6000;

    protected float nextWaypointDistance = 1.5f;
    protected float roamPointRadius = 4f;

    protected int currentWaypoint = 0;
    protected bool reachedEndOfPath;

    protected float pathCalculationRate = .2f;
    protected float pathCalculationTimer;
    #endregion

    protected Vector3 moveDir;
    protected float moveSpeed;

    protected virtual void Start() {
        seeker = GetComponent<Seeker>();
        //Initialise path
        CalculatePath(transform.position);
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

        moveDir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 velocity = moveDir * moveSpeed * speedFactor;

        if (!reachedEndOfPath) {
            transform.position += velocity * Time.deltaTime;
        }
    }

    protected void CalculatePath(Vector3 destinationPoint) {
        seeker.StartPath(transform.position, destinationPoint, PathComplete);
    }

    protected void CalculateFleePath(Vector3 fleePosition) {
        // Create a path object
        FleePath path = FleePath.Construct(transform.position, fleePosition, theGScoreToStopAt);

        // This is how strongly it will try to flee, if you set it to 0 it will behave like a RandomPath
        path.aimStrength = 1f;

        // Determines the variation in path length that is allowed
        path.spread = 1000;

        seeker.StartPath(path, FleePathComplete);
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

    public Vector3 GetMoveDir() {
        return moveDir;
    }

    public bool GetReachedEndOfPath() {
        return reachedEndOfPath;
    }

    
}
