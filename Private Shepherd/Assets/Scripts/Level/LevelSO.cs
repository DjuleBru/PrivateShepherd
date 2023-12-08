using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelSO : ScriptableObject
{
    public string levelName;
    public int levelBoneUnlockCost;
    public int levelTimeLimit;
    public int bronzeScoreTreshold;
    public int silverScoreTreshold;
    public int goldScoreTreshold;
    public int platScoreTreshold;

    public int timeScore;

    public int sheepNumber;
    public int whiteSheepNumber;
    public int blackSheepNumber;
    public int blueSheepNumber;
    public int greenSheepNumber;
    public int redSheepNumber;
    public int goldSheepNumber;
    public int goatSheepNumber;

    public AudioClip levelMusic;
    public AudioClip dangerMusic;
}
