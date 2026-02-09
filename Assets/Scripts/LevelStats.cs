using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelStats : ScriptableObject
{
    [Header("Stage1")]
    public int Stage1Time;
    public bool isTimed1;

    [Header("Stage2")]
    public int Stage2Time;
    public bool isTimed2;

    [Header("Stage3")]
    public int Stage3Time;
    public bool isTimed3;

    [Header("Stage4")]
    public int Stage4Time;
    public bool isTimed4;
}
