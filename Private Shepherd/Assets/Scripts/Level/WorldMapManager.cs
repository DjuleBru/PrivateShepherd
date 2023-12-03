using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public static WorldMapManager Instance { get; private set; }

    [SerializeField] private Transform playerInitialPosition;

    private void Awake() {
        Instance = this; 
    }

    void Start()
    {
        Player.Instance.SetPlayerPosition(ES3.Load("playerWorldMapPosition", playerInitialPosition.position));
        Player.Instance.GetComponent<PlayerBark>().SetBarkActive(true);
        Player.Instance.GetComponent<PlayerGrowl>().SetGrowlActive(true);
        Player.Instance.GetComponent<PlayerRun>().SetRunActive(true);
    }

    public void SavePlayerPosition() {
        ES3.Save("playerWorldMapPosition", Player.Instance.transform.position);
    }

}
