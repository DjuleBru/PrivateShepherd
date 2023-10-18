using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldMapUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI bonesText;

    public void Start() {
        bonesText.text = Player.Instance.GetPlayerBones().ToString();
    }
}
