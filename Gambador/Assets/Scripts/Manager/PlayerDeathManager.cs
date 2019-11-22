using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathManager
{
    [HideInInspector] public delegate void PlayerDeathEvent();
    [HideInInspector] public static event PlayerDeathEvent OnPlayerDeath;
    private GameObject player;
    public static bool PlayerIsDead;

    public PlayerDeathManager()
    {
        player = GameObject.Find("Player");
        OnPlayerDeath += PlayerDeath;
    }

    public void PlayerDeathTrigger()
    {
        OnPlayerDeath?.Invoke();
    }

    private void PlayerDeath()
    {
        PlayerIsDead = true;
        player.transform.position = PlayerManager.respawnArea;
    }
}
