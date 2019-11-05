using System.Collections;
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
    [HideInInspector] public delegate void PlayerMovingEventManager();
    [HideInInspector] public static event GameEventManager SystemOnInit;

    [HideInInspector] public static event GameEventManager ApplicationOnQuit;
    [HideInInspector] public static event GameEventManager ApplicationOnPause;
    [HideInInspector] public static event GameEventManager ApplicationOnFocus;

    [HideInInspector] public static event GameEventManager GameUpdate;
    [HideInInspector] public static event GameEventManager GameFixedUpdate;
    [HideInInspector] public static event PlayerMovingEventManager PlayerMovingStart;
    [HideInInspector] public static event PlayerMovingEventManager PlayerMovingStop;


    // Declare all your service here
    [HideInInspector] public CameraManager CameraManager { get; set; }
    [HideInInspector] public RaycastManager RaycastManager { get; set; }
    [HideInInspector] public MovingPlayerManager MovingPlayerManager { get; set; }
    public void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            Debug.Log("singleton:" + singleton.ToString() + " is created");
        }
        else if (singleton != null && singleton != this)
        {
            Debug.LogWarning("singleton:" + singleton.ToString());
            Debug.LogWarning("GameManager Gameobject, OnAwake : Singleton already assigned. Need to destroy this gameobject.");
            Destroy(this);
            return;
        }
        StartGameManager();
    }
    private void StartGameManager()
    {
        try
        {
            // define your services here
            CameraManager = new CameraManager("Main Camera");
            RaycastManager = new RaycastManager();
            MovingPlayerManager = new MovingPlayerManager("Player");

            DontDestroyOnLoad(this.gameObject);
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
        if (GameUpdate != null)
            GameUpdate();
        if (PlayerMovingStart != null)
            PlayerMovingStart();
        if (PlayerMovingStart != null)
            PlayerMovingStop();
    }

    private void OnMouseDown()
    {
        
    }

    private void FixedUpdate()
    {
        if (GameFixedUpdate != null)
            GameFixedUpdate();
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
        if (ApplicationOnQuit != null)
            ApplicationOnQuit();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (ApplicationOnFocus != null)
                ApplicationOnFocus();
        }
        else
        {
            Debug.Log("Unfocus");
            if (ApplicationOnPause != null)
                ApplicationOnPause();
        }
    }


    public void DestroyServices()
    {
        StopAllCoroutines();
        DestroyAllManagers();
        DestroyAllClients();
        DestroyAllListeners();
        coroutines = null;
    }

    private void DestroyAllManagers()
    {
        CameraManager = null;
        RaycastManager = null;
        MovingPlayerManager = null;
    }
    private void DestroyAllClients()
    {

    }

    private void DestroyAllListeners()
    {

    }
}