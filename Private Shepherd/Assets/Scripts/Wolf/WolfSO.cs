using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WolfSO : ScriptableObject
{
    public float roamPauseMaxTime;
    public float roamPauseMinTime;

    public float fleeSpeed;
    public float roamSpeed;
    public float attackSpeed;
}
