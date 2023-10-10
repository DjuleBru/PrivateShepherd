using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrowlUI : MonoBehaviour
{
    [SerializeField] Image growlCoolDownImage;

    private void Update() {
        growlCoolDownImage.fillAmount = PlayerGrowl.Instance.GetGrowlCoolDownTimerNormalized();
    }
}
