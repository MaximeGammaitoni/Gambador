using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlayerManager : BaseGameObjectManager
{
    private MeshRenderer playerMesh;
    private BoxCollider playerCollider;
    private Rigidbody playerRigidBody;
    private ParticleSystem particleSystem;
    private GameObject laser;
    private float speed;
    private bool canMove = true;

    [HideInInspector] public delegate void PlayerMovingEventManager();

    [HideInInspector] public static event PlayerMovingEventManager PlayerMovingEventStart;
    [HideInInspector] public static event PlayerMovingEventManager PlayerMovingEventStop;

    public MovingPlayerManager(string gameObjectName) : base(gameObjectName)
    {
        speed = Config.PlayerSpeed;
        playerMesh = mainGameObject.GetComponent<MeshRenderer>();
        playerCollider = mainGameObject.GetComponent<BoxCollider>();
        playerRigidBody = mainGameObject.GetComponent<Rigidbody>();
        laser = mainGameObject.transform.Find("Laser").gameObject;
        particleSystem = mainGameObject.transform.Find("Particle").GetComponent<ParticleSystem>();
        particleSystem.Stop();
    }

    public void StartMovingPlayer(Vector3 dest)
    {
        GameManager.singleton.StartCouroutineInGameManager(MovingPlayerCoroutine(dest));

    }

    private void TriggerStartMovingPlayer()
    {
        PlayerMovingEventStart?.Invoke();
    }

    private void TriggerStopMovingPlayer()
    {
        PlayerMovingEventStop?.Invoke();
    }

    IEnumerator MovingPlayerCoroutine(Vector3 destination)
    {
        //il faudra créé un propriété dans le game object player bool "invinsible"
        
        if (canMove)
        {
            TriggerStartMovingPlayer();
            var distance = destination - mainGameObject.transform.position;
            var vulnerabilityStartDistance = distance * Config.StartVulnerabilityInRunRatio;
            var vulnerabilityBeforeStopDistance = distance * Config.EndVulnerabilityInRunRatio;
            var invulnerabilityDistance = (distance - vulnerabilityStartDistance) - vulnerabilityBeforeStopDistance;
            var initialPos = mainGameObject.transform.position;
            canMove = false;
            laser.SetActive(false);

            while (mainGameObject.transform.position != initialPos + vulnerabilityStartDistance)
            {
                mainGameObject.transform.position = Vector3.MoveTowards(mainGameObject.transform.position, initialPos + vulnerabilityStartDistance, speed * Time.deltaTime * Config.TimeScale);
                yield return null;
            }


            playerMesh.enabled = false;
            particleSystem.Play();
            

           
            initialPos = mainGameObject.transform.position;
            while (mainGameObject.transform.position != initialPos + invulnerabilityDistance)
            {
                mainGameObject.transform.position = Vector3.MoveTowards(mainGameObject.transform.position, initialPos + invulnerabilityDistance, speed * Time.deltaTime * Config.TimeScale);
                yield return null;
            }

            
            
            playerMesh.enabled = true;
            particleSystem.Stop();
            float ratioSpeed = 0;
            initialPos = mainGameObject.transform.position;
            while (mainGameObject.transform.position != initialPos + vulnerabilityBeforeStopDistance)
            {
                mainGameObject.transform.position = Vector3.Lerp(mainGameObject.transform.position, initialPos + vulnerabilityBeforeStopDistance, speed * ratioSpeed /30);
                ratioSpeed += Time.deltaTime * Config.TimeScale;
                yield return null;
            }
            TriggerStopMovingPlayer();
            laser.SetActive(true);
            canMove = true;
        }
    }
}
