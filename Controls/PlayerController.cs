using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    /*
	 * This script goes in the player (which has to have tag Player).
	 * Players needs another collider but with the IsTrigger activated.
	 * This controls basic behaviour of the player
	 **/

    private Animator animator; // References the animator
    public GameObject movementJoystick; // Get the game object which controls the player movement.
    private Rigidbody rigidBody; // Player's rigidbody.
    private VirtualJoystick mover; // To get the input vector.
    public GameObject spellCanvasPrefab;
    private GameObject spellCanvas;
    public Image locator;

    private Timer timer;
    public int movementSpeed; // Movement speed.
    private int originalSpeed;
    public int dashForce; // Throwback speed.
    public int force; // Used to calculate the strength of the character, linked to how much "damage"
                      // it does on collision.
    public bool isDashing; // Says if the character is in a dash attack
    private bool isRunning;
    //public GameObject countdown;

    public float stunTime; // Time the player will be stunned.
    public bool isStunned; // Says whether the player is stunned.

    // Unity coroutines
    private IEnumerator speedDown;
    private IEnumerator stunEnu;
    private IEnumerator dashEnu;

    private GlobalVariables globals; // An empty object with all the global variables in it.

    private int myPlayer; // An integer indicating which player the script is referring to.

    [SyncVar]
    public int gameStarted;

    private bool serverNotSet = true;
    void Start()
    {
        timer = GetComponent<Timer>();
        isRunning = false;

        animator = GetComponent<Animator>();
        isStunned = false;

        rigidBody = this.GetComponent<Rigidbody>();
        mover = movementJoystick.GetComponent<VirtualJoystick>();

        globals = GameObject.Find("GlobalVariables").GetComponent<GlobalVariables>();
        myPlayer = globals.currentSpawnedPlayer;

        gameObject.tag = globals.tags[myPlayer]; // Set a tag to the player depending on the order it spawned

        globals.currentSpawnedPlayer++;
        // Store the speed of the player (to use when reducing speed or so)
        originalSpeed = movementSpeed;

        isDashing = false;

        gameStarted = 0;

        if (isLocalPlayer)
        {
            spellCanvas = Instantiate(spellCanvasPrefab) as GameObject;
            spellCanvas.transform.SetParent(transform);

            switch (tag)
            {
                case "PlayerPurple":
                    locator.color = Color.magenta;
                    break;
                case "PlayerRed":
                    locator.color = Color.red;
                    break;
                case "PlayerBlue":
                    locator.color = Color.cyan;
                    break;
                case "PlayerGreen":
                    locator.color = Color.green;
                    break; ;
            }
            locator.enabled = true;
        }
    }

    void Update()
    {
        if (GameObject.Find("PanelPreferencesIngame") != null && serverNotSet)
        {
            Debug.Log("SERVER CHECKED");
            GameObject.Find("PanelPreferencesIngame").GetComponent<PauseMenu>().server = isServer;
            GameObject.Find("PanelPreferencesIngame").GetComponent<PauseMenu>().myPlayer = this;

            serverNotSet = false;
        }

        if (isServer && globals.allowPlayerMovement)
        {
            gameStarted = 1;
        }

        if (isLocalPlayer)
        {
            if (isDashing)
            {
                Dash();
                Invoke("DashReset", 0.15f);
            }
            // Controls the behaviour of the player depending on if it's stunned.
            if (!isStunned)
            {
                // Moves the player
                Move();
            }
            else
            {
                stunEnu = DoStun();
                // Player loses the ball if on possession.
                if (tag == GetComponent<BallHandler>().hasTheBall &&
                    GetComponent<BallHandler>().hasTheBall != null)
                {
                    GetComponent<BallHandler>().CmdUngrabBall();
                }
                // Animation
                animator.SetBool("isRunning", false);
                animator.Play("Stun");
                // Start the stun coroutine
                StartCoroutine(stunEnu);
            }
        }
    }
    //Funcion que se llama en todos los jugadores, clientes y host
    [ClientRpc]
    public void RpcKickEveryone()
    {
        NetworkManager m = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>().manager;
        if (isServer)
        {
            m.matchMaker.DestroyMatch(m.matches[0].networkId, 0, OnDestroyMatch1);
        }
        else
        {
            m.matchMaker.DestroyMatch(m.matches[0].networkId, 0, OnDestroyMatch2);
        }
        Destroy(GameObject.Find("mc"));
        SceneManager.LoadScene("MainMenu");
    }

    //OnDestroyMatch Para el Servidor
    public void OnDestroyMatch1(bool success, string extendedInfo)
    {
        Debug.Log("SERVER PLAYER DESTROYED");
        NetworkManager.singleton.StopHost();
        NetworkManager.singleton.StopMatchMaker();
        NetworkManager.Shutdown();
        Destroy(GameObject.Find("NetworkManager"));
        NetworkTransport.Shutdown();
    }

    //OnDestroyMatch Para el Cliente
    public void OnDestroyMatch2(bool success, string extendedInfo)
    {
        Debug.Log("CLIENT PLAYER DESTROYED");
        NetworkManager.singleton.StopMatchMaker();
        NetworkManager.Shutdown();
        Destroy(GameObject.Find("NetworkManager"));
    }

    private void DashReset()
    {
        isDashing = false;
    }

    // This function is called to set the fireball spell animation
    public void PlaySpellAnimation()
    {
        //animator.Play("Spell");
    }

    //This function is called when a player is stunned
    private IEnumerator DoStun()
    {
        yield return new WaitForSeconds(stunTime);
        // Stun time is over.
        isStunned = false;
        GetComponent<Stamina>().StopStun();
    }

    private void Dash()
    {
        GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().velocity +
            GetComponent<Rigidbody>().transform.forward * dashForce);
    }

    // Called to move the player depending on the input vector.
    private void Move()
    {
        if (timer.allowPlayerMovementTimer)
        {
            mover.inputVector = GameObject.Find("Joystick").GetComponent<VirtualJoystick>().inputVector;
            transform.Translate(mover.inputVector * movementSpeed * Time.deltaTime, Space.World);

            if (mover.inputVector != Vector3.zero)
            {
                if (!isRunning)
                {
                    if (GetComponent<PedradaPlayer>() == null)
                    {
                        animator.Play("Running");
                    }
                }
                isRunning = true;
                transform.rotation = Quaternion.LookRotation(mover.inputVector);
                if (GetComponent<PedradaPlayer>() == null)
                {
                    animator.SetBool("isRunning", isRunning);
                }
            }
            else
            {
                isRunning = false;
                // Stop the running animation
                //Check Cauac
                if (GetComponent<PedradaPlayer>() == null)
                {
                    animator.Play("Idle");
                    animator.SetBool("isRunning", isRunning);
                }
            }
        }
    }

    private IEnumerator ReduceSpeed(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        movementSpeed = originalSpeed;
    }

    void OnCollisionEnter(Collision col)
    {
        // When hit by another player that is dashing and this player is not
        if (col.gameObject.tag.Contains("Player"))
        {
            // Player loses the ball if on possession.
            // if (tag == GetComponent<BallHandler>().hasTheBall && GetComponent<BallHandler>().hasTheBall != null)
            // {
            //     GetComponent<BallHandler>().CmdUngrabBall();
            // }
            // Reduce Stamina on collision
            GetComponent<Stamina>().TakeDamage(col.collider.GetComponent<PlayerController>().force);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Ice":
                if (other.GetComponent<IceLifetime>().owner != gameObject.tag)
                {
                    speedDown = ReduceSpeed(5.0f);
                    movementSpeed = movementSpeed / 2;
                    animator.speed = 0.5f;
                    StartCoroutine(speedDown);
                }
                break;
        }
    }

    // Called every time player is hit by a particle system.
    void OnParticleCollision(GameObject other)
    {
        switch (other.tag)
        {
            case "FireBall":
                if (other.GetComponent<FireballLifetime>().owner != gameObject.tag)
                {
                    GetComponent<Stamina>().TakeDamage(other.GetComponent<FireballLifetime>().damage);
                }
                break;
            case "Avalanche":
                if (other.GetComponent<PedradaLifetime>().owner != gameObject.tag)
                {
                    GetComponent<Stamina>().TakeDamage(other.GetComponent<PedradaLifetime>().damage);
                }
                break;
            case "Boulder":
                if (other.GetComponent<IndianaBolaLifetime>().owner != gameObject.tag)
                {
                    GetComponent<Stamina>().TakeDamage(other.GetComponent<IndianaBolaLifetime>().damage);
                }
                break;
        }
    }
}