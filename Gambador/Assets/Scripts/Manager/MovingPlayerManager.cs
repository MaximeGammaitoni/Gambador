using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlayerManager : BaseGameObjectManager
{
    private GameObject playerMesh;
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
        playerMesh = mainGameObject.transform.Find("PlayerMesh").gameObject;
        playerCollider = mainGameObject.GetComponent<BoxCollider>();
        playerRigidBody = mainGameObject.GetComponent<Rigidbody>();
        laser = mainGameObject.transform.Find("Laser").gameObject;
        particleSystem = mainGameObject.transform.Find("Particle").GetComponent<ParticleSystem>();
        particleSystem.Stop();
        PlayerDeathManager.OnPlayerDeath += () => { canMove = false; };
        PlayerDeathManager.OnPlayerRevive += () => { canMove = true; };
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
                if (PlayerDeathManager.PlayerIsDead)
                    break;
                mainGameObject.transform.position = Vector3.MoveTowards(mainGameObject.transform.position, initialPos + vulnerabilityStartDistance, speed * Time.deltaTime * Config.TimeScale);
                yield return null;
            }


            playerMesh.SetActive(false);
            particleSystem.Play();
            

           
            initialPos = mainGameObject.transform.position;
            while (mainGameObject.transform.position != initialPos + invulnerabilityDistance)
            {
                if (PlayerDeathManager.PlayerIsDead)
                    break;
                mainGameObject.transform.position = Vector3.MoveTowards(mainGameObject.transform.position, initialPos + invulnerabilityDistance, speed * Time.deltaTime * Config.TimeScale);
                yield return null;
            }

            
            
            playerMesh.SetActive(true);

            playerMesh.transform.rotation = Quaternion.LookRotation(-invulnerabilityDistance, Vector3.up);
            RaycastHit hit;
            if (Physics.Raycast(playerMesh.transform.position, - playerMesh.transform.up,out hit ,0.2f))
            {
                playerMesh.transform.rotation = Quaternion.Lerp(playerMesh.transform.rotation, Quaternion.LookRotation(Vector3.Cross(playerMesh.transform.right, hit.normal), hit.normal), 100);
            }
            particleSystem.Stop();
            float ratioSpeed = 0;
            initialPos = mainGameObject.transform.position;
            while (mainGameObject.transform.position != initialPos + vulnerabilityBeforeStopDistance)
            {
                if (PlayerDeathManager.PlayerIsDead)
                    break;
                mainGameObject.transform.position = Vector3.Lerp(mainGameObject.transform.position, initialPos + vulnerabilityBeforeStopDistance, speed * ratioSpeed /30);
                ratioSpeed += Time.deltaTime * Config.TimeScale;
                yield return null;
            }
            TriggerStopMovingPlayer();
            GameManager.singleton.AttackManager.AttackEnemy(destination, "Circular");
            laser.SetActive(true);
            canMove = true;
        }
    }
}
