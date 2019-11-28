using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config 
{
    public static float TimeScale = 1;
    public const string PlayerTag = "Player";
    public static readonly float PlayerSpeed = 50;
    public const  float AttackRange = 4;
    public static readonly float StartVulnerabilityInRunRatio = 0.0f ; //dans un dash au début, le ratio ou le player est vulnerable
    public static readonly float EndVulnerabilityInRunRatio = 0.0f; //dans un dash a la fin, meme chose 
    public static readonly float TimeBeforeRevive = 1f;
}
