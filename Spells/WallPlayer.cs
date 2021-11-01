using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WallPlayer : NetworkBehaviour
{
    public GameObject audioOnline;
    public GameObject particleSys; // The actual particle system for the spell
    public int damage; // Amount of damage the fireball does
    private float angle;

    private void Update()
    {
        if (isLocalPlayer)
        {
            foreach (Transform t in transform)
            {
                if (t.name == "SpellCanvas(Clone)")
                {
                    angle = t.GetChild(2).transform.GetChild(0).GetComponent<WallController>().angle;
                    Aim(angle);
                }
            }
        }
    }

    public void Aim(float angle)
    {
        particleSys.transform.eulerAngles = new Vector3(0, angle + 90, 0);
        particleSys.transform.position = transform.position + new Vector3(Mathf.Cos(angle * Mathf.PI / 180) * 3, 0.35f, -Mathf.Sin(angle * Mathf.PI / 180) * 3);

    }

    public void PlayWall()
    {
        if (isLocalPlayer)
        {

            GetComponent<PlayerController>().PlaySpellAnimation();
            CmdPlayWall(particleSys.transform.position, particleSys.transform.eulerAngles);
        }
    }

    [Command]
    public void CmdPlayWall(Vector3 position, Vector3 rotation)
    {
        RpcPlayWall(position, rotation);
    }

    [ClientRpc]
    public void RpcPlayWall(Vector3 position, Vector3 rotation)
    {
        // Play only if the player is not stunned
        if (!(GetComponent<PlayerController>().isStunned))
        {
            GameObject audio = Instantiate(audioOnline) as GameObject;
            audio.GetComponent<AudioPlayerOnline>().playfire();

            GameObject particleSysNetwork = Instantiate(particleSys) as GameObject;
            particleSysNetwork.transform.eulerAngles = rotation;
            particleSysNetwork.transform.position = position;
            //Play the particle system
            particleSysNetwork.GetComponent<ParticleSystem>().Play();
        }
    }
}