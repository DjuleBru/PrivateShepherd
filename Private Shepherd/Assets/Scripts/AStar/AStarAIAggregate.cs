using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAIAggregate : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private Sheep sheep;
    private Transform closestSheepTransform; 

    private Seeker seeker;
    public Path path;

    // The path will be returned when the path is over a specified length (or more accurately when the traversal cost is greater than a specified value).
    // A score of 1000 is approximately equal to the cost of moving one world unit.
    private float nextWaypointDistance = 1f;

    private float speed = 2;
    private int currentWaypoint = 0;

    private bool reachedEndOfPath;

    void Start() {
        seeker = GetComponent<Seeker>();
        Player.Instance.OnPlayerMovedOverTreshold += Player_OnPlayerMovedOverTreshold;
    }

    private void Player_OnPlayerMovedOverTreshold(object sender, System.EventArgs e) {
        // Player moved over a certain distance treshold
        closestSheepTransform = sheep.GetClosestSheep();
        seeker.StartPath(transform.position, closestSheepTransform.position, MyCompleteFunction);
    }

    public void Update() {

        if (path == null) {
            return;
        }

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

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 velocity = dir * speed * speedFactor;

        if (!reachedEndOfPath) {
            transform.position += velocity * Time.deltaTime;
        }

    }

    public void MyCompleteFunction(Path p) {
        path = p;
        currentWaypoint = 0;
    }
}
