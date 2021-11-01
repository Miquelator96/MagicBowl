using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallHandler : NetworkBehaviour
{
    public GameObject ballPrefab; // Ball's prefab
    private GameObject ball; // This is where the ball is instantiated
    public string hasTheBall; // This string says which player has the ball
    public bool ballIsGrabbed = false; // Boolean to know whether the ball is grabbed or not
    public Vector3 spawnPosition; // Position in which the ball will respawn

    void Start()
    {
        // If host, spawn the ball
        if (isServer && isLocalPlayer)
        {
            var spawnRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            ball = Instantiate(ballPrefab, spawnPosition, spawnRotation) as GameObject;
            NetworkServer.Spawn(ball);
        }
    }

    //Grab the ball when someone collides with the ball
    private void OnTriggerEnter(Collider collider)
    {
        if (isLocalPlayer && collider.tag == "Ball" && ballIsGrabbed == false)
        {
            CmdGrabBall(gameObject);
        }
    }

    // This method is called on the host, and makes the player grab the ball
    [Command]
    public void CmdGrabBall(GameObject player)
    {
        RpcGrabBall(player);
    }

    // Tell all the clients who grabbed the ball
    [ClientRpc]
    void RpcGrabBall(GameObject player)
    {
        if (ball == null)
        {
            ball = GameObject.FindWithTag("Ball");
        }
        ball.GetComponent<Rigidbody>().useGravity = false;
        ball.transform.SetParent(player.transform);
        ball.transform.position = player.transform.position + new Vector3(0.0f, 2.5f, 0.0f);
        hasTheBall = player.tag;
        ballIsGrabbed = true;
    }

    // This method is called on the host, and makes the player ungrab the ball - if he has it
    [Command]
    public void CmdUngrabBall()
    {
        RpcUngrabBall();
    }

    // Tell all the clients that the ball is free
    [ClientRpc]
    public void RpcUngrabBall()
    {
        if (ball == null)
        {
            ball = GameObject.FindWithTag("Ball");
        }
        ball.transform.SetParent(null);
        if (ball.transform.position.x >= 0)
        {
            ball.transform.position -= new Vector3(1.5f, 0.0f, 0.0f);
        }
        else
        {
            ball.transform.position += new Vector3(1.5f, 0.0f, 0.0f);
        }
        Vector3 newPos = ball.transform.position;
        newPos.y = 0.5f;
        ball.transform.position = newPos;
        ballIsGrabbed = false;
        hasTheBall = null;
    }

    // This method is called on the host, and respawns the ball at its original position
    [Command]
    public void CmdRespawnBall()
    {
        RpcRespawnBall();
    }

    // Tell all the clients that the ball has respawned
    [ClientRpc]
    public void RpcRespawnBall()
    {
        if (ball == null)
        {
            ball = GameObject.FindWithTag("Ball");
        }
        // Ball respawns
        ball.transform.SetParent(null);
        ball.transform.position = spawnPosition;
        // Stop the ball from following the player
        ballIsGrabbed = false;
        hasTheBall = null;
    }
}
