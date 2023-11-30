using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSpawner : MonoBehaviour
{
    [SerializeField] GameObject tentacleModel;
    [SerializeField] List<Transform> spawnPoints;

    private void Start() {
        int i = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[i];
        GameObject tentacle = Instantiate(tentacleModel, spawnPoint.transform.position, Quaternion.identity);
        tentacle.gameObject.SetActive(true);
    }
}
