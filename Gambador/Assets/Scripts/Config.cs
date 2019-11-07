using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config 
{
    public static float TimeScale = 1;

    public static readonly float PlayerSpeed = 50; 
    public static readonly float Range = 7; //range initial
    public static readonly float RangeIncrementBy = 1; //  la range augmente apres chaque combo 
    public static readonly float MaxRange = 50; // bloque avant d'atteindre cette range
    public static readonly float StartVulnerabilityInRunRatio = 0 ; //dans un dash au début, le ratio ou le player est vulnerable
    public static readonly float EndVulnerabilityInRunRatio = 0.5f; //dans un dash a la fin, meme chose 
}
