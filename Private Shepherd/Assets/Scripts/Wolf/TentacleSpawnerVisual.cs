using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSpawnerVisual : MonoBehaviour
{
    private void Start() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }
}
