using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IndianaBolaPlayer : NetworkBehaviour
{
    public GameObject audioOnline;
    public GameObject particleSys; // The actual particle system for the spell
    public int damage; // Amount of damage the stone does
    private float angle;

    private void Update()
    {
        if (isLocalPlayer)
        {
            foreach (Transform t in transform)
            {
                if (t.name == "SpellCanvasCauac(Clone)")
                {
                    angle = t.GetChild(0).transform.GetChild(0).GetComponent<IndianaBolaController>().angle;
                    Aim(angle);
                }
            }
        }
    }

    public void Aim(float angle)
    {
        particleSys.transform.eulerAngles = new Vector3(0, angle, 0);
        particleSys.transform.position = transform.position + new Vector3(0, 0.5f, 0);
    }

    public void PlayIndianaBola()
    {
        if (isLocalPlayer)
        {
            GetComponent<PlayerController>().PlaySpellAnimation();
            CmdPlayIndianaBola(particleSys.transform.position, particleSys.transform.eulerAngles);
        }
    }

    [Command]
    public void CmdPlayIndianaBola(Vector3 position, Vector3 rotation)
    {
        RpcPlayIndianaBola(position, rotation);
    }

    [ClientRpc]
    public void RpcPlayIndianaBola(Vector3 position, Vector3 rotation)
    {
        // Play only if the player is not stunned
        if (!(GetComponent<PlayerController>().isStunned))
        {
            GameObject audio = Instantiate(audioOnline) as GameObject;
            audio.GetComponent<AudioPlayerOnline>().playindiana();

            particleSys.GetComponent<IndianaBolaLifetime>().owner = gameObject.tag;
            GameObject particleSysNetwork = Instantiate(particleSys) as GameObject;
            particleSysNetwork.transform.eulerAngles = rotation;
            particleSysNetwork.transform.position = position;
            //Play the particle system
            particleSysNetwork.GetComponent<ParticleSystem>().Play();

            //Cal probar si funciona, si s'executa el so a l'altra banda
        }
    }
}