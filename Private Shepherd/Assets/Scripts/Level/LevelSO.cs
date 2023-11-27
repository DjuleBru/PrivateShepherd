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

    public int whiteSheepScore;
    public int goldSheepScore;
    public int blackSheepScore;
    public int blueSheepScore;
    public int redSheepScore;
    public int greenSheepScore;
    public int timeScore;

    public int sheepNumber;
}
