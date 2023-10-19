using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldMapUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI bonesText;

    public void Start() {
        Player.Instance.OnBonesChanged += Player_OnBonesChanged;
        bonesText.text = Player.Instance.GetPlayerBones().ToString();
    }

    private void Player_OnBonesChanged(object sender, System.EventArgs e) {
        bonesText.text = Player.Instance.GetPlayerBones().ToString();
    }
}
