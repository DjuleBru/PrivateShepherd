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
}
