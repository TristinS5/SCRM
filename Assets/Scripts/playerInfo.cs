using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class playerInfo : ScriptableObject
{
    [Header("Health")]
    public int MaxHP;
    public int CurrentHP;

    [Header("Speed")]
    public int origSpeed;
    public int Speed;
    public int sprintMod;

    [Header("Jump")]
    public int jumpMax;
    public int jumpForce;
    public int Gravity;
}
