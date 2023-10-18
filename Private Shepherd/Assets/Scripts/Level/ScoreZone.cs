using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    [SerializeField] private Transform[] aggregatePointArray;
    [SerializeField] private PolygonCollider2D scoreZoneCollider;

    public Transform[] GetAggregatePointArray() { 
        return aggregatePointArray; 
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject collisionGameObject = collision.gameObject;

        if (collisionGameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
            sheep.SheepEnteredScoreZone(this);
        }
    }

}
