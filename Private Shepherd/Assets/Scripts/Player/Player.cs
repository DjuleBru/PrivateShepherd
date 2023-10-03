using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerBark))]

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
}
