using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SFXSO : ScriptableObject
{

    #region Sheep
    public AudioClip[] sheepBaa;
    public AudioClip[] sheepBaaAggregate;
    public AudioClip[] sheepEnterScoreZone;
    public AudioClip[] sheepDie;
    public AudioClip[] goatRam;
    public AudioClip[] goatRamWarning;
    #endregion

    #region ENEMY
    public AudioClip[] wolfBites;
    public AudioClip[] wolfEats;
    public AudioClip[] wolfFlee;
    public AudioClip[] tentacleEmerge;
    public AudioClip[] tentacleHits;
    public AudioClip[] tentacleEats;
    #endregion

    #region UI
    public AudioClip[] UIButtonHover;
    public AudioClip[] UIButtonClick;
    #endregion

    #region NPC
    public AudioClip[] NPCLetterShow;
    #endregion

    #region PLAYER
    public AudioClip[] playerGrowl;
    public AudioClip[] playerBark;
    public AudioClip playerSneak;
    public AudioClip playerRun;
    public AudioClip playerMove;
    public AudioClip playerTired;
    #endregion



}
