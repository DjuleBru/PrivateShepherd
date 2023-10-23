using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

public class AStarRecalculateGraph : MonoBehaviour
{

    private void Update() {
        UpdateGraph();
    }
    private void UpdateGraph() {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        AstarPath.active.UpdateGraphs(bounds);
    }
}
