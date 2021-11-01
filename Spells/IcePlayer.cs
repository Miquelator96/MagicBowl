using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IcePlayer : NetworkBehaviour
{
	public GameObject audioOnline;
    public GameObject particleSys;
    
    public void PlayIce()
    {
        if (isLocalPlayer)
        {

            GetComponent<PlayerController>().PlaySpellAnimation();
            CmdPlayIce(transform.position + new Vector3(0, 0.1f, 0));
        }
    }

    [Command]
    public void CmdPlayIce(Vector3 position)
    {
        RpcPlayIce(position);
    }

    [ClientRpc]
    public void RpcPlayIce(Vector3 position)
    {
        if (!(GetComponent<PlayerController>().isStunned))
        {
			GameObject audio = Instantiate(audioOnline) as GameObject;
			audio.GetComponent<AudioPlayerOnline> ().playice ();

            // Set the owner of the particle system so that the spell doesn't affect the player
            particleSys.GetComponent<IceLifetime>().owner = gameObject.tag;
            GameObject particleSysNetwork = Instantiate(particleSys) as GameObject;
            particleSysNetwork.transform.position = position;
            foreach (Transform child in particleSysNetwork.transform)
            {
				child.GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
