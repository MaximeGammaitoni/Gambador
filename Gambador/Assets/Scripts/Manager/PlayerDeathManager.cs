using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathManager
{
    [HideInInspector] public delegate void PlayerDeathEvent();
    [HideInInspector] public static event PlayerDeathEvent OnPlayerDeath;
    [HideInInspector] public static event PlayerDeathEvent OnPlayerRevive;
    private GameObject player;
    public static bool PlayerIsDead;

    public PlayerDeathManager()
    {
        player = GameObject.Find("Player");
    }

    public void PlayerDeathTrigger()
    {
        GameManager.singleton.StartCoroutine(DeathPlayerAnim());
        OnPlayerDeath?.Invoke();
       
    }
    public void PlayerRevivTrigger()
    {
        PlayerIsDead = false;
        OnPlayerRevive?.Invoke();
       
    }
    IEnumerator DeathPlayerAnim()
    {
        PlayerIsDead = true;
        player.transform.position = PlayerManager.respawnArea;
        yield return new WaitForSeconds(Config.TimeBeforeRevive);
       
        PlayerRevivTrigger();
    }
}
