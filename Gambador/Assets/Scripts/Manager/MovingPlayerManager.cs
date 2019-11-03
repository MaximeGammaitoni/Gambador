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
    private float speed = 40;
    private bool canMove = true;
    public MovingPlayerManager(string gameObjectName) : base(gameObjectName)
    {
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

    IEnumerator MovingPlayerCoroutine(Vector3 destination)
    {
        //il faudra créé un propriété dans le game object player bool "invinsible"

        if (canMove)
        {
            canMove = false;

            playerMesh.enabled = false;
            particleSystem.Play();
            laser.SetActive(false);

            while (mainGameObject.transform.position != destination)
            {
                mainGameObject.transform.position = Vector3.MoveTowards(mainGameObject.transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }

            playerMesh.enabled = true;
            particleSystem.Stop();
            laser.SetActive(true);
            canMove = true;
        }
    }
}
