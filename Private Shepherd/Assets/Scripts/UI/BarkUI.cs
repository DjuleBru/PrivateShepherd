using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarkUI : MonoBehaviour
{
    [SerializeField] Image barkCoolDownImage;

    private void Update() {
        barkCoolDownImage.fillAmount = PlayerBark.Instance.GetBarkCoolDownTimerNormalized();
    }
}
