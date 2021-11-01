using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PedradaPlayer : NetworkBehaviour
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
                    angle = t.GetChild(1).transform.GetChild(0).GetComponent<PedradaController>().angle;
                    Aim(angle);
                }
            }
        }
    }

    public void Aim(float angle)
    {
        particleSys.transform.eulerAngles = new Vector3(90, 0, 0);
        particleSys.transform.position = transform.position + new Vector3(Mathf.Cos(angle*Mathf.PI/180)*3,3, 
            -Mathf.Sin(angle * Mathf.PI / 180) * 3);
    }

    public void PlayPedrada()
    {
        if (isLocalPlayer)
        {
            GetComponent<PlayerController>().PlaySpellAnimation();
            CmdPlayPedrada(particleSys.transform.position, particleSys.transform.eulerAngles);
        }
    }

    [Command]
    public void CmdPlayPedrada(Vector3 position, Vector3 rotation)
    {
        RpcPlayPedrada(position, rotation);
    }

    [ClientRpc]
    public void RpcPlayPedrada(Vector3 position, Vector3 rotation)
    {
        // Play only if the player is not stunned
        if (!(GetComponent<PlayerController>().isStunned))
        {
            GameObject audio = Instantiate(audioOnline) as GameObject;
            audio.GetComponent<AudioPlayerOnline>().playpedrada();

            particleSys.GetComponent<PedradaLifetime>().owner = gameObject.tag;
            GameObject particleSysNetwork = Instantiate(particleSys) as GameObject;
            particleSysNetwork.transform.eulerAngles = rotation;
            particleSysNetwork.transform.position = position;
            //Play the particle system
            particleSysNetwork.GetComponent<ParticleSystem>().Play();

            //Cal probar si funciona, si s'executa el so a l'altra banda
        }
    }
}