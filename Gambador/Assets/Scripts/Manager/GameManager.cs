﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager singleton;
    [HideInInspector] public Dictionary<string, IEnumerator> coroutines;
    [HideInInspector] public delegate void GameEventManager();
    [HideInInspector] public static event GameEventManager SystemOnInit;

    [HideInInspector] public static event GameEventManager ApplicationOnQuit;
    [HideInInspector] public static event GameEventManager ApplicationOnPause;
    [HideInInspector] public static event GameEventManager ApplicationOnFocus;

    [HideInInspector] public static event GameEventManager GameUpdate;
    [HideInInspector] public static event GameEventManager GameFixedUpdate;


    // Declare all your service here
    [HideInInspector] public CameraManager CameraManager { get; set; }
    [HideInInspector] public RaycastManager RaycastManager { get; set; }
    [HideInInspector] public MovingPlayerManager MovingPlayerManager { get; set; }
    [HideInInspector] public EnemyManager EnemyManager { get; set; }
    [HideInInspector] public AttackManager AttackManager { get; set; }
    [HideInInspector] public CursorManager CursorManager { get; set; }
    [HideInInspector] public SoundManager SoundManager { get; set; }

    [HideInInspector] public PlayerDeathManager PlayerDeathManager { get; set; }

    public void Awake()
    {
        GameUpdate = null;
        GameFixedUpdate = null;
        singleton = this;
        Debug.Log("singleton:" + singleton.ToString() + " is created");
        StartGameManager();
    }
    private void StartGameManager()
    {
        try
        {
           
            // define your services here
            
            RaycastManager = new RaycastManager();
            EnemyManager = new EnemyManager();
            AttackManager = new AttackManager();
            MovingPlayerManager = new MovingPlayerManager("Player");
            CursorManager = new CursorManager();
            SoundManager = new SoundManager();
            PlayerDeathManager = new PlayerDeathManager();
            CameraManager = new CameraManager("Main Camera");

            SFXManager.SetSfx();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    public void OnDisable()
    {

        //TODO : disable other game event
    }

    public void InitUnitySystem()
    {
        if (SystemOnInit != null)
        {
            Debug.Log(" GAME MANAGER INIT UNITY SYSTEM");
            SystemOnInit();
        }
    }

    private void Update()
    {
        GameUpdate?.Invoke();

    }

    private void OnMouseDown()
    {
       
    }

    private void FixedUpdate()
    {
        GameFixedUpdate?.Invoke();
    }
    public void StartCouroutineInGameManager(IEnumerator routine, string routineName)
    {
        if (coroutines == null)
        {
            coroutines = new Dictionary<string, IEnumerator>();
        }
        if (coroutines != null && !coroutines.ContainsKey(routineName))
        {
            Coroutine co = StartCoroutine(routine);
            coroutines.Add(routineName, routine);
        }
        else if (coroutines != null && coroutines.ContainsKey(routineName))
        {
            StopCouroutineInGameManager(routineName);
            Coroutine l_co = StartCoroutine(routine);
            coroutines.Add(routineName, routine);
        }
    }
    public void StartCouroutineInGameManager(IEnumerator routine)//Coroutine avec arret automatique du MonoBehavior
    {
        StartCoroutine(routine);
    }
    public void StopCouroutineInGameManager(string coroutineName)
    {
        if (coroutines.ContainsKey(coroutineName))
        {
            StopCoroutine(coroutines[coroutineName]);
            coroutines.Remove(coroutineName);
        }
    }

    void OnApplicationQuit()
    {
        ApplicationOnQuit?.Invoke();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            ApplicationOnFocus?.Invoke();
        }
        else
        {
            ApplicationOnPause?.Invoke();
        }
    }

    public void DestroyServices()
    {
        StopAllCoroutines();
        DestroyAllManagers();
        DestroyAllClients();
        DestroyAllListeners();
        coroutines = null;
        //System.Web.HttpRuntime.UnloadAppDomain();
    }

    private void DestroyAllManagers()
    {
        // define your services here
        CameraManager = null;
        RaycastManager =null;
        EnemyManager = null;
        AttackManager =null;
        MovingPlayerManager =null;
        CursorManager =null;
        SoundManager = null;
        PlayerDeathManager =  null;
    }
    private void DestroyAllClients()
    {

    }

    private void DestroyAllListeners()
    {

    }
}