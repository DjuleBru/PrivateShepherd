using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayerUnlockAbilitiesUI : MonoBehaviour
{

    [SerializeField] GameObject unlockPanelUI;

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description1;
    [SerializeField] TextMeshProUGUI description2;
    [SerializeField] TextMeshProUGUI description3;
    [SerializeField] TextMeshProUGUI description4;

    [SerializeField] Image buttonIcon;

    [SerializeField] VideoClip barkClip;
    [SerializeField] VideoClip growlClip;
    [SerializeField] VideoClip runClip;
    [SerializeField] VideoClip sneakClip;
    [SerializeField] VideoPlayer player;

    public bool QuestGiverCinematicOver;
    [SerializeField] private QuestGiver[] questGivers;

    private bool growlUnlocked;
    private bool runUnlocked;
    private bool sneakUnlocked;
    private bool barkUnlocked;

    void Start() {
        unlockPanelUI.SetActive(false);
        PlayerUnlockAbilities.Instance.OnBarkUnlocked += Instance_OnBarkUnlocked;
        PlayerUnlockAbilities.Instance.OnRunUnlocked += Instance_OnRunUnlocked;
        PlayerUnlockAbilities.Instance.OnSneakUnlocked += Instance_OnSneakUnlocked;
        PlayerUnlockAbilities.Instance.OnGrowlUnlocked += Instance_OnGrowlUnlocked;

        foreach (QuestGiver questGiver in questGivers) {
            questGiver.OnQuestGiversCinematicOver += QuestGiver_OnQuestGiversCinematicOver;
        }
    }

    private void Update() {
        if(barkUnlocked) {
            if(!QuestGiverCinematicOver) {
                return;
            } else {
                StartCoroutine(UnlockBark());
                barkUnlocked = false;
            }
        }

        if (sneakUnlocked) {
            if (!QuestGiverCinematicOver) {
                return;
            }
            else {
                StartCoroutine(UnlockSneak());
                sneakUnlocked = false;
            }
        }

        if (runUnlocked) {
            if (!QuestGiverCinematicOver) {
                return;
            }
            else {
                StartCoroutine(UnlockRun());
                runUnlocked = false;
            }
        }

        if (growlUnlocked) {
            if (!QuestGiverCinematicOver) {
                return;
            }
            else {
                StartCoroutine(UnlockGrowl());
                growlUnlocked = false;
            }
        }
    }

    private void QuestGiver_OnQuestGiversCinematicOver(object sender, System.EventArgs e) {
        QuestGiverCinematicOver = true;
    }

    private void Instance_OnGrowlUnlocked(object sender, System.EventArgs e) {
        growlUnlocked = true;
    }

    private void Instance_OnSneakUnlocked(object sender, System.EventArgs e) {
        sneakUnlocked = true;
    }

    private void Instance_OnRunUnlocked(object sender, System.EventArgs e) {
        runUnlocked = true;
    }

    private void Instance_OnBarkUnlocked(object sender, System.EventArgs e) {
        barkUnlocked = true;
    }

    private IEnumerator UnlockGrowl() {

        DeActivatePlayer();
        unlockPanelUI.SetActive(true);
        player.clip = growlClip;

        title.text = "Growl";
        description1.text = "Hold          to growl";
        description2.text = "• Makes wolves Flee";
        description3.text = "• Doesn't affect sheep";
        description4.text = "• Has a limited duration";

        buttonIcon.sprite = InputSpriteManager.Instance.GetGrowlSprite();
        buttonIcon.transform.localScale = InputSpriteManager.Instance.GetGrowlSpriteScale();
        yield return null;
    }
    private IEnumerator UnlockSneak() {

        DeActivatePlayer();
        unlockPanelUI.SetActive(true);
        player.clip = sneakClip;

        title.text = "Sneak";
        description1.text = "Hold          to sneak";
        description2.text = "• Sheep don't flee while sneaking";
        description3.text = "";
        description4.text = "";

        buttonIcon.sprite = InputSpriteManager.Instance.GetSneakSprite();
        buttonIcon.transform.localScale = InputSpriteManager.Instance.GetSneakSpriteScale();
        yield return null;
    }
    private IEnumerator UnlockBark() {
        DeActivatePlayer();
        unlockPanelUI.SetActive(true);
        player.clip = barkClip;

        title.text = "Bark";
        description1.text = "Predd          to bark";
        description2.text = "• Makes wolves flee";
        description2.text = "• Makes sheep flee";
        description4.text = "• Makes Sheep move faster";

        buttonIcon.sprite = InputSpriteManager.Instance.GetBarkSprite();
        buttonIcon.transform.localScale = InputSpriteManager.Instance.GetBarkSpriteScale();
        yield return null;
    }
    private IEnumerator UnlockRun() {
        DeActivatePlayer();
        unlockPanelUI.SetActive(true);
        player.clip = runClip;

        title.text = "Run";
        description1.text = "Hold          to run";
        description2.text = "• Increases movement speed";
        description3.text = "• Get tired if you run too long";
        description4.text = "";

        buttonIcon.sprite = InputSpriteManager.Instance.GetRunSprite();
        buttonIcon.transform.localScale = InputSpriteManager.Instance.GetRunSpriteScale();
        yield return null;
    }

    private void DeActivatePlayer() {
        PlayerMovement.Instance.SetCanMove(false);
        PlayerBark.Instance.SetBarkActive(false);
        PlayerGrowl.Instance.SetGrowlActive(false);
    }

    private void ActivatePlayer() {
        PlayerMovement.Instance.SetCanMove(true);
        PlayerBark.Instance.SetBarkActive(true);
        PlayerGrowl.Instance.SetGrowlActive(true);
    }

    public void RemoveUI() {
        unlockPanelUI.SetActive(false);
        ActivatePlayer();
    }
}
